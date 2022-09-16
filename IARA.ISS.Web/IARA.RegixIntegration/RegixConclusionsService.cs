using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.BatchWorkers.Interfaces;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration
{
    public class RegixConclusionsService : IRegixConclusionsService
    {
        private readonly IAsyncWorkerTaskQueue<RegixConclusionContext, bool> workerQueue;
        private readonly ScopedServiceProviderFactory scopedServiceProviderFactory;
        private readonly ConcurrentDictionary<RegixConclusionContext, int> workingTasks;
        public RegixConclusionsService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connectionString)
        {
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
            this.workingTasks = new ConcurrentDictionary<RegixConclusionContext, int>();
            this.workerQueue = WorkerCreationUtils.CreateWorkerQueue<RegixConclusionContext, bool>(TryUpdateApplicationStatusExternal, connectionString, nameof(RegixConclusionsService));
        }

        public Task<bool> FinalDecision(int applicationId, int applicationHistoryId)
        {
            RegixConclusionContext context = new RegixConclusionContext
            {
                ApplicationId = applicationId,
                ApplicationHistoryId = applicationHistoryId
            };

            return workerQueue.Enqueue(context);
        }


        private Task<bool> TryUpdateApplicationStatusExternal(RegixConclusionContext data, CancellationToken token)
        {
            if (workingTasks.TryAdd(data, 0))
            {
                try
                {
                    return TryUpdateApplicationStatus(data, token);
                }
                finally
                {
                    workingTasks.TryRemove(data, out _);
                }
            }

            return Task.FromResult(false);
        }


        private Task<bool> TryUpdateApplicationStatus(RegixConclusionContext data, CancellationToken token)
        {
            try
            {
                using (IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    using (IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>())
                    {
                        db.ApplyComplexAudit = false;
                        var allApplicationChecks = (from check in db.ApplicationRegiXchecks
                                                    where check.ApplicationId == data.ApplicationId
                                                       && check.ApplicationChangeHistoryId == data.ApplicationHistoryId
                                                    select new
                                                    {
                                                        check.ResponseStatus,
                                                        check.ErrorLevel,
                                                        check.ErrorDescription
                                                    }).ToList();

                        ApplicationStatusesEnum status = (from application in db.Applications
                                                          join applStatus in db.NapplicationStatuses on application.ApplicationStatusId equals applStatus.Id
                                                          where application.Id == data.ApplicationId
                                                          select Enum.Parse<ApplicationStatusesEnum>(applStatus.Code)).First();

                        bool responseStatus = allApplicationChecks.All(x => x.ResponseStatus == RegixResponseStatusEnum.Cache.ToString()
                                                                         || x.ResponseStatus == RegixResponseStatusEnum.OK.ToString());

                        if (status == ApplicationStatusesEnum.EXT_CHK_STARTED && responseStatus)
                        {
                            IApplicationStateMachine applicationStateMachine = serviceProvider.GetRequiredService<IApplicationStateMachine>();

                            if (allApplicationChecks.Any(x => x.ErrorLevel == RegixCheckStatusesEnum.ERROR.ToString()))
                            {
                                string errorDescription = allApplicationChecks
                                                          .Where(x => x.ErrorLevel == RegixCheckStatusesEnum.ERROR.ToString())
                                                          .Select(x => x.ErrorDescription)
                                                          .First();

                                status = applicationStateMachine.Act(data.ApplicationId, null, errorDescription);
                            }
                            else if (allApplicationChecks.Any(x => x.ErrorLevel == RegixCheckStatusesEnum.WARN.ToString()))
                            {
                                status = applicationStateMachine.Act(data.ApplicationId, ApplicationStatusesEnum.INSP_CORR_FROM_EMP);
                            }
                            else
                            {
                                bool isTicket = (from appl in db.Applications
                                                 join type in db.NapplicationStatusHierarchyTypes on appl.ApplicationStatusHierTypeId equals type.Id
                                                 where appl.Id == data.ApplicationId
                                                 select type.Code).First() == ApplicationHierarchyTypesEnum.RecreationalFishingTicket.ToString();

                                if (isTicket)
                                {
                                    status = applicationStateMachine.Act(data.ApplicationId);
                                }
                                else
                                {
                                    status = applicationStateMachine.Act(data.ApplicationId, ApplicationStatusesEnum.INSP_CORR_FROM_EMP);
                                }
                            }

                            return Task.FromResult(true);
                        }
                        else if (status == ApplicationStatusesEnum.EXT_CHK_STARTED && !responseStatus)
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
                using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
                IExtendedLogger logger = serviceProvider.GetRequiredService<IExtendedLogger>();
                logger.LogException(ex);
            }

            return Task.FromResult(false);
        }

    }
}
