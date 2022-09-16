using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.FVMSModels;
using IARA.FVMSModels.CrossChecks;
using IARA.FVMSModels.ExternalModels;
using IARA.FVMSModels.GeoZones;
using IARA.FVMSModels.NISS;
using IARA.Infrastructure.FluxIntegrations.QueueServices;
using IARA.Interfaces.FVMSIntegrations;
using IARA.Logging.Abstractions.Interfaces;
using TL.BatchWorkers.Interfaces;
using TL.SysToSysSecCom;
using TLTTS.Common.ConfigModels;

namespace IARA.Infrastructure.FVMSIntegrations
{
    public class FVMSQueueService : BaseFluxService, IFVMSReceiverIntegrationService, IFVMSInitiatorIntegrationService
    {
        private IAsyncWorkerTaskQueue<CCheckReport, bool> crossCheckReportQueue;
        private IAsyncWorkerTaskQueue<List<Permit>, bool> permitChangeQueue;
        private IAsyncWorkerTaskQueue<List<TelemetryStatus>, bool> telemetryQueue;

        public FVMSQueueService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
            : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.permitChangeQueue = WorkerCreationUtils.CreateWorkerQueue<List<Permit>, bool>(HandlePermitChange, connection, nameof(permitChangeQueue));
            this.crossCheckReportQueue = WorkerCreationUtils.CreateWorkerQueue<CCheckReport, bool>(HandleCrossCheckReport, connection, nameof(crossCheckReportQueue));
            this.telemetryQueue = WorkerCreationUtils.CreateWorkerQueue<List<TelemetryStatus>, bool>(HandleTelemetryData, connection, nameof(telemetryQueue));
        }

        public Task<bool> EnqueuePermitChange(Permit permit)
        {
            return permitChangeQueue.Enqueue(new List<Permit> { permit });
        }

        public Task<bool> EnqueuePermitsChange(List<Permit> permits)
        {
            return permitChangeQueue.Enqueue(permits);
        }

        public List<FishingGear> GetFishingGears(NISSQuery query)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IPermitsAndLicencesService service = serviceProvider.GetService<IPermitsAndLicencesService>();

            return service.GetFishingGears(query.Identifier);
        }

        public GeoZoneReport GetGeoZoneReport(GeoZoneQuery query)
        {
            throw new NotImplementedException();
        }

        public Certificate GetLicense(NISSQuery query)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IPermitsAndLicencesService service = serviceProvider.GetService<IPermitsAndLicencesService>();

            if (query.Type == NISSQueryType.LicenseByNumber)
            {
                return service.GetLicense(query.Identifier);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(query.Type));
            }
        }

        public List<Certificate> GetLicenses(NISSQuery query)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IPermitsAndLicencesService service = serviceProvider.GetService<IPermitsAndLicencesService>();

            switch (query.Type)
            {
                case NISSQueryType.LicenseByCFR:
                    return service.GetLicensesByCFR(query.Identifier);
                case NISSQueryType.LicenseByPermNumber:
                    return service.GetLicensesByPermitNumber(query.Identifier);
                default:
                    throw new ArgumentOutOfRangeException(nameof(query.Type));
            }
        }

        public Permit GetPermit(NISSQuery query)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IPermitsAndLicencesService service = serviceProvider.GetService<IPermitsAndLicencesService>();

            switch (query.Type)
            {
                case NISSQueryType.PermByNumber:
                    return service.GetPermit(query.Identifier);
                default:
                    throw new ArgumentOutOfRangeException(nameof(query.Type));
            }
        }

        public List<Permit> GetPermitsByCFR(NISSQuery query)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IPermitsAndLicencesService service = serviceProvider.GetService<IPermitsAndLicencesService>();

            switch (query.Type)
            {
                case NISSQueryType.PermByCFR:
                    return service.GetPermits(query.Identifier);
                default:
                    throw new ArgumentOutOfRangeException(nameof(query.Type));
            }
        }

        public CCheckReport ReceiveCCheckQuery(CCheckQuery query)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IFVMSCrossCheckService service = serviceProvider.GetService<IFVMSCrossCheckService>();
            return service.ReceiveQuery(query);
        }

        public Task<bool> ReceiveCrossCheckReport(CCheckReport report)
        {
            return this.crossCheckReportQueue.Enqueue(report);
        }

        public Task<bool> ReceiveTelemetryData(List<TelemetryStatus> telemetryData)
        {
            return this.telemetryQueue.Enqueue(telemetryData);
        }

        public Task<List<TelemetryStatus>> GetVesselTelemetries(TelemetryQuery query, CancellationToken? token = null)
        {
            query.Identifier = Guid.NewGuid();

            return SendMessageToFvms<TelemetryQuery, List<TelemetryStatus>>(query.Identifier, 
                                                                            query, 
                                                                            nameof(FluxFvmsDomainsEnum.TelemDomain), 
                                                                            FVMSEndpoints.TELEMETRY_QUERY, 
                                                                            token);
        }

        public Task<List<TelemetryStatus>> GetTelemetries(List<TelemetryQuery> queries, CancellationToken? token = null)
        {
            return SendMessageToFvms<List<TelemetryQuery>, List<TelemetryStatus>>(Guid.NewGuid(), 
                                                                                  queries, 
                                                                                  nameof(FluxFvmsDomainsEnum.TelemDomain),
                                                                                  FVMSEndpoints.MULTIPLE_TELEMETRY_QUERIES,
                                                                                  token);
        }

        private Task<bool> HandleCrossCheckReport(CCheckReport report, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IFVMSCrossCheckService service = serviceProvider.GetService<IFVMSCrossCheckService>();

            try
            {
                service.ReceiveReport(report);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                IExtendedLogger logger = serviceProvider.GetRequiredService<IExtendedLogger>();
                logger.LogException(ex);
                return Task.FromResult(false);
            }
        }

        private Task<bool> HandlePermitChange(List<Permit> permits, CancellationToken token)
        {
            if (permits != null && permits.Count > 0)
            {
                if (permits.Count == 1)
                {
                    return SendPermit(permits[0], token);
                }
                else
                {
                    return SendPermits(permits, token);
                }
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        private Task<bool> HandleTelemetryData(List<TelemetryStatus> telemetryData, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            ITelemetryService service = serviceProvider.GetRequiredService<ITelemetryService>();

            try
            {
                service.WriteTelemetry(telemetryData);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                IExtendedLogger logger = serviceProvider.GetRequiredService<IExtendedLogger>();
                logger.LogException(ex);
                return Task.FromResult(false);
            }
        }

        private async Task<bool> SendMessageToFvms<T>(Guid requestId, 
                                                      T message, 
                                                      string domainName,
                                                      string url,
                                                      CancellationToken? token = null)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();

            try
            {
                using (ISecureHttpClient secureHttpClient = serviceProvider.GetRequiredService<ISecureHttpClient>())
                {
                    secureHttpClient.BaseAddress = new Uri(ExternalSystemSettings.Default.FvmsBaseUrl);
                    AddOrUpdateRequest(requestId, message, true, domainName, url);

                    if (token != null && token.HasValue)
                    {
                        HttpResponseMessage response = await secureHttpClient.SendAsync(url, message, token, ensureSuccessStatus: true);
                    }
                    else
                    {
                        HttpResponseMessage response = await secureHttpClient.SendAsync(url, message, ensureSuccessStatus: true);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);

                if (requestId != Guid.Empty)
                {
                    AddRequestError(requestId, ex.Message, serviceProvider);
                }

                return false;
            }
        }

        private async Task<TResponse> SendMessageToFvms<TRequest, TResponse>(Guid requestId, 
                                                                             TRequest message, 
                                                                             string domainName, 
                                                                             string url, 
                                                                             CancellationToken? token = null)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();

            try
            {
                using (ISecureHttpClient secureHttpClient = serviceProvider.GetRequiredService<ISecureHttpClient>())
                {
                    secureHttpClient.BaseAddress = new Uri(ExternalSystemSettings.Default.FvmsBaseUrl);
                    AddOrUpdateRequest(requestId, message, true, domainName, url);

                    if (token != null && token.HasValue)
                    {
                        return await secureHttpClient.SendAsync<TRequest, TResponse>(url, message, token);
                    }
                    else
                    {
                        return await secureHttpClient.SendAsync<TRequest, TResponse>(url, message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);

                if (requestId != Guid.Empty)
                {
                    AddRequestError(requestId, ex.Message, serviceProvider);
                }

                throw;
            }
        }

        private Task<bool> SendPermit(Permit perm, CancellationToken token)
        {
            return Retry(() => SendMessageToFvms(Guid.NewGuid(),
                                                 perm, 
                                                 nameof(FluxFvmsDomainsEnum.PLGDomain), 
                                                 FVMSEndpoints.NISS_PERMIT,
                                                 token));
        }

        private Task<bool> SendPermits(List<Permit> permits, CancellationToken token)
        {
            return Retry(() => SendMessageToFvms(Guid.NewGuid(), 
                                                 permits, 
                                                 nameof(FluxFvmsDomainsEnum.PLGDomain),
                                                 FVMSEndpoints.MULTIPLE_NISS_PERMITS, 
                                                 token));
        }
    }
}
