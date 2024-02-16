using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.Interfaces;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Models;
using Microsoft.Extensions.DependencyInjection;
using TL.BatchWorkers.Abstractions.Interfaces.Queue;
using TL.Dependency.Abstractions;
using TL.Logging.Abstractions.Interfaces;
using Timer = System.Timers.Timer;

namespace IARA.RegixIntegration
{
    [ServiceScope(ServiceLifetime.Singleton)]
    public class RegixNewConclusionsService : IRegixConclusionsService
    {
        private const int MINUTES_TO_DISCARD = 20;

        private readonly IExtendedLoggerSingleton logger;
        private readonly object padlock;
        private readonly IScopedServiceProviderFactory scopedServiceProviderFactory;
        private readonly Timer timer;
        private readonly IAsyncWorkerTaskQueue<RegixConclusionContext, bool> workerQueue;
        private ConcurrentDictionary<int, RegixConclusionContext> conclusionContexts;

        public RegixNewConclusionsService(IScopedServiceProviderFactory scopedServiceProviderFactory, IExtendedLoggerSingleton logger)
        {
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
            this.logger = logger;
            this.timer = new Timer();
            this.padlock = new object();
            this.conclusionContexts = new ConcurrentDictionary<int, RegixConclusionContext>();
            this.workerQueue = WorkerCreationUtils.CreateWorkerQueue<RegixConclusionContext, bool>(TryUpdateApplicationStatus, null, nameof(RegixConclusionsService), true, logger: logger);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Interval = TimeSpan.FromSeconds(RegixConclusionContext.END_PERIOD_SECONDS / 2).TotalMilliseconds;
            this.timer.Enabled = true;
        }

        public Task<bool> FinalDecision(int applicationId, int applicationHistoryId)
        {
            var context = conclusionContexts.AddOrUpdate(applicationId, (key) => new RegixConclusionContext()
            {
                ApplicationId = applicationId,
                ApplicationHistoryId = applicationHistoryId
            }, (key, value) =>
            {
                value.ApplicationHistoryId = applicationHistoryId;

                return value;
            });

            return this.workerQueue.Enqueue(context);
        }

        public void FixHangedRegixChecks(int minutesBefore)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IARADbContext db = serviceProvider.GetService<IARADbContext>();
            IApplicationService applicationService = serviceProvider.GetService<IApplicationService>();

            List<ApplicationContext> hangedApplicationsList = (from app in db.Applications
                                                               join appHistory in db.ApplicationChangeHistories on app.Id equals appHistory.ApplicationId
                                                               join check in db.ApplicationRegiXchecks on app.Id equals check.ApplicationId
                                                               join status in db.NapplicationStatuses on app.ApplicationStatusId equals status.Id
                                                               where appHistory.ValidTo == DefaultConstants.MAX_VALID_DATE
                                                                  && app.IsActive
                                                                  && status.Code == nameof(ApplicationStatusesEnum.EXT_CHK_STARTED)
                                                                  && check.RequestDateTime < DateTime.Now.AddMinutes(-minutesBefore)
                                                                  && check.ApplicationChangeHistoryId == appHistory.Id
                                                               select new ApplicationCheck
                                                               {
                                                                   ApplicationId = app.Id,
                                                                   ApplicationHistoryId = appHistory.Id,
                                                                   CheckStatus = Enum.Parse<RegixCheckStatusesEnum>(check.ErrorLevel, true),
                                                                   ResponseStatus = Enum.Parse<RegixResponseStatusEnum>(check.ResponseStatus, true)
                                                               }).ToList()
                                                                 .GroupBy(x => new { x.ApplicationId, x.ApplicationHistoryId })
                                                                 .Select(x => new ApplicationContext(x))
                                                                 .ToList();


            foreach (var hangedApplication in hangedApplicationsList)
            {
                if (!this.conclusionContexts.ContainsKey(hangedApplication.Id))
                {
                    if (!hangedApplication.IsFailed())
                    {
                        applicationService.ConfirmNoErrorsForApplication(hangedApplication.Id);
                    }
                    else
                    {
                        applicationService.FallbackApplicationToEdit(hangedApplication.Id);
                    }
                }
            }
        }

        private ApplicationContext GetApplicationChecks(IARADbContext db, int applicationId, int applicationHistoryId)
        {
            var checks = (from check in db.ApplicationRegiXchecks
                          join application in db.Applications on check.ApplicationId equals application.Id
                          join hierarchiType in db.NapplicationStatusHierarchyTypes on application.ApplicationStatusHierTypeId equals hierarchiType.Id
                          where check.ApplicationId == applicationId
                             && check.ApplicationChangeHistoryId == applicationHistoryId
                          select new ApplicationCheck
                          {
                              ApplicationId = applicationHistoryId,
                              ApplicationHistoryId = applicationHistoryId,
                              ResponseStatus = Enum.Parse<RegixResponseStatusEnum>(check.ResponseStatus),
                              CheckStatus = Enum.Parse<RegixCheckStatusesEnum>(check.ErrorLevel),
                              ErrorDescription = check.ErrorDescription,
                              ApplicationHierarchyType = Enum.Parse<ApplicationHierarchyTypesEnum>(hierarchiType.Code)
                          }).ToList();

            return new ApplicationContext(checks);
        }

        private ApplicationStatusesEnum GetApplicationStatus(IARADbContext db, int applicationId)
        {
            return (from application in db.Applications
                    join applStatus in db.NapplicationStatuses on application.ApplicationStatusId equals applStatus.Id
                    where application.Id == applicationId
                    select Enum.Parse<ApplicationStatusesEnum>(applStatus.Code)).First();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(padlock))
            {
                try
                {
                    using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
                    IARADbContext db = serviceProvider.GetService<IARADbContext>();

                    foreach (var regixContext in conclusionContexts.Values)
                    {
                        TimeSpan difference = (DateTime.Now - regixContext.EnqueuedTime);

                        if (difference > TimeSpan.FromMinutes(MINUTES_TO_DISCARD))
                        {
                            this.conclusionContexts.TryRemove(regixContext.ApplicationId, out _);
                        }
                        else if (difference > TimeSpan.FromSeconds(RegixConclusionContext.END_PERIOD_SECONDS))
                        {
                            TryUpdateApplicationStatus(regixContext, CancellationToken.None).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogException(ex);
                }
                finally
                {
                    Monitor.Exit(padlock);
                }
            }
        }
        private Task<bool> TryUpdateApplicationStatus(RegixConclusionContext data, CancellationToken token)
        {
            try
            {
                using (IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    using (IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>())
                    {
                        ApplicationContext context = GetApplicationChecks(db, data.ApplicationId, data.ApplicationHistoryId);
                        ApplicationStatusesEnum status = GetApplicationStatus(db, data.ApplicationId);

                        if (!context.IsResponseMissing())
                        {
                            this.conclusionContexts.TryRemove(context.Id, out _);

                            if (status == ApplicationStatusesEnum.EXT_CHK_STARTED)
                            {
                                IApplicationService applicationService = serviceProvider.GetRequiredService<IApplicationService>();

                                if (!context.IsFailed())
                                {
                                    status = applicationService.ConfirmNoErrorsForApplication(context.Id);
                                }
                                else
                                {
                                    status = applicationService.FallbackApplicationToEdit(context.Id);
                                }
                            }
                            else
                            {
                                logger?.LogWarning($"Application status has changed in the middle of procedure ApplicationID: {data.ApplicationId}");
                            }
                        }
                        else if (context.IsResponseMissing() && status == ApplicationStatusesEnum.EXT_CHK_STARTED)
                        {
                            //FALLBACK FUNCTIONALITY
                            TimeSpan delay = data.CalculateDelay();
                            if (delay != TimeSpan.Zero)
                            {
                                workerQueue.Enqueue(data, timeToDelay: delay);
                                return Task.FromResult(true);
                            }
                            else
                            {
                                return Task.FromResult(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogException(ex, callerFileName: "RegixConcluctionsService", callerName: "TryUpdateApplicationStatus");

                try
                {
                    using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
                    IApplicationService applicationService = serviceProvider.GetRequiredService<IApplicationService>();
                    applicationService.FallbackApplicationToEdit(data.ApplicationId);
                }
                catch (Exception e)
                {
                    logger?.LogException(e);
                }
            }

            return Task.FromResult(false);
        }
    }
}
