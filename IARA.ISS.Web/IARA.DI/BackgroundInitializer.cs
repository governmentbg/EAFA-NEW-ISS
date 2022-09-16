using System;
using IARA.Common.ConfigModels;
using IARA.Common.Constants;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.CrossChecks;
using Microsoft.Extensions.DependencyInjection;
using TL.BatchWorkers.Schedulers;
using TL.BatchWorkers.Utils;

namespace IARA.DI
{
    internal static class BackgroundInitializer
    {
        public static void AddBackgroundWorkers(this IServiceCollection services)
        {
            services.AddSingleton<NewHostedServiceScheduler>();
            services.AddHostedService<NewHostedServiceScheduler>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<StartupSettings>().BackgroundTasks;
                var scheduler = serviceProvider.GetRequiredService<NewHostedServiceScheduler>();

                if (settings.Enabled)
                {
                    if (settings.SendMobileNotifications)
                    {
                        scheduler.AddRepeatingTask(serviceProvider => TaskUtils.ExecuteTask<INewsManagementService>(serviceProvider, service =>
                        {
                            service.SendMobileNotifications();
                        }, DefaultConstants.BACKGROUND_TASK_USER), TimeSpan.FromMinutes(30), "Repeating_MobileNotifications");
                    }

                    if (settings.UnlockUsers)
                    {
                        scheduler.AddRepeatingTask(serviceProvider => TaskUtils.ExecuteTask<IUserService>(serviceProvider, service =>
                        {
                            service.TryUnlockUsers();
                        }, DefaultConstants.BACKGROUND_TASK_USER), TimeSpan.FromMinutes(5), "Repeating_UnlockUsers");
                    }

                    if (settings.InactivatePastTickets)
                    {
                        scheduler.AddEveryDayTask(provider => TaskUtils.ExecuteTask<IFishingTicketService>(serviceProvider, service =>
                        {
                            service.InactivatePastTickets();
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(00, 00, 00), "EveryDay_InactiveTickets");
                    }

                    if (settings.ACDRReporting)
                    {
                        scheduler.AddMontlyTask(provider => TaskUtils.ExecuteTask<IAggregatedCatchReportService>(serviceProvider, service =>
                        {
                            service.ReportAggregatedCatches();
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(12, 00, 30, 00), "Montly_ACDR");
                    }

                    if (settings.DailyCrossChecks)
                    {
                        scheduler.AddEveryDayTask(provider => TaskUtils.ExecuteTask<ICrossChecksExecutionService>(serviceProvider, service =>
                        {
                            service.ExecuteCrossChecks("Daily");
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(01, 00, 00), "Daily_CrossChecks");
                    }

                    if (settings.WeeklyCrossChecks)
                    {
                        scheduler.AddWeeklyTask(provider => TaskUtils.ExecuteTask<ICrossChecksExecutionService>(serviceProvider, service =>
                        {
                            service.ExecuteCrossChecks("Weekly");
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(10, 50, 00), DayOfWeek.Thursday, "Weekly_CrossChecks");
                    }

                    if (settings.MontlyCrossChecks)
                    {
                        scheduler.AddMontlyTask(provider => TaskUtils.ExecuteTask<ICrossChecksExecutionService>(serviceProvider, service =>
                        {
                            service.ExecuteCrossChecks("Montly");
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(01, 02, 00, 00), "Montly_CrossChecks");
                    }

                    if (settings.PermitsSuspensionFlagUpdate)
                    {
                        scheduler.AddEveryDayTask(provider => TaskUtils.ExecuteTask<ICommercialFishingService>(serviceProvider, service =>
                        {
                            service.UpdatePermitsIsSuspendedFlag();
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(0, 20, 0), "EveryDay_PermitSuspensionFlag");
                    }

                    if (settings.PermitLicensesSuspensionFlagUpdate)
                    {
                        scheduler.AddEveryDayTask(provider => TaskUtils.ExecuteTask<ICommercialFishingService>(serviceProvider, service =>
                        {
                            service.UpdatePermitLicensesIsSuspendedFlag();
                        }, DefaultConstants.BACKGROUND_TASK_USER), new TimeSpan(0, 40, 0), "EveryDay_PermitLicenseSuspensionFlag");
                    }
                }
                return scheduler;
            });
        }
    }
}
