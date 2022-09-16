using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.Logging.Abstractions.Interfaces;
using TL.BatchWorkers.Interfaces;
using TL.SysToSysSecCom;
using TLTTS.Common.ConfigModels;

namespace IARA.Infrastructure.FluxIntegrations.QueueServices
{
    public abstract class BaseFluxService
    {
        protected const byte MAX_RETRY_COUNT = 3;

        protected readonly IAsyncWorkerTaskQueue<FLUXResponseMessageType, bool> responseQueue;
        protected readonly ScopedServiceProviderFactory scopedServiceProviderFactory;
        protected BaseFluxService(ScopedServiceProviderFactory scopedServiceProviderFactory, 
                                  ConnectionStrings connection, 
                                  string queueName)
        {
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
            this.responseQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXResponseMessageType, bool>(SaveResponse, 
                                                                                                      connection, 
                                                                                                      queueName);
        }

        public virtual Task<bool> ReceiveResponse(FLUXResponseMessageType response)
        {
            return responseQueue.Enqueue(response);
        }

        protected bool AddOrUpdateRequest<TRequest>(FLUXReportDocumentType report, 
                                                    TRequest request, 
                                                    bool isOutgoing, 
                                                    string domainName,
                                                    string webServiceUrl, 
                                                    IScopedServiceProvider serviceProvider = null)
        {
            IDType id = FindReportDocumentId(report);
            return AddOrUpdateRequest(id, request, isOutgoing, domainName, webServiceUrl, serviceProvider);
        }

        protected bool AddOrUpdateRequest<TRequest>(IDType id, 
                                                    TRequest request, 
                                                    bool isOutgoing, 
                                                    string domainName,
                                                    string webServiceUrl, 
                                                    IScopedServiceProvider serviceProvider = null)
        {
            Guid requestId = ParseGUID(id);
            return AddOrUpdateRequest(requestId, request, isOutgoing, domainName, webServiceUrl, serviceProvider);
        }

        protected bool AddOrUpdateRequest<TRequest>(Guid id, 
                                                    TRequest request, 
                                                    bool isOutgoing, 
                                                    string domainName,
                                                    string webServiceUrl,
                                                    IScopedServiceProvider serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                return AddOrUpdateRequestPrivate(id, request, isOutgoing, domainName, webServiceUrl, serviceProvider);
            }
            else
            {
                using (IScopedServiceProvider localServiceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    return AddOrUpdateRequestPrivate(id, request, isOutgoing, domainName, webServiceUrl, localServiceProvider);
                }
            }
        }

        protected bool AddRequestError(FLUXReportDocumentType document, string errorDescription, IScopedServiceProvider serviceProvider = null)
        {
            IDType id = FindReportDocumentId(document);

            return AddRequestError(ParseGUID(id), errorDescription, serviceProvider);
        }

        protected bool AddRequestError(FLUXResponseDocumentType document, string errorDescription, IScopedServiceProvider serviceProvider = null)
        {
            IDType id = FindReportDocumentId(document);

            return AddRequestError(ParseGUID(id), errorDescription, serviceProvider);
        }

        protected bool AddRequestError(IDType requestId, string errorDescription, IScopedServiceProvider serviceProvider = null)
        {
            Guid id = ParseGUID(requestId);
            return AddRequestError(id, errorDescription, serviceProvider);
        }

        protected bool AddRequestError(Guid requestId, string errorDescription, IScopedServiceProvider serviceProvider = null)
        {
            if (requestId != Guid.Empty)
            {
                if (serviceProvider != null)
                {
                    return AddRequestErrorPrivate(requestId, errorDescription, serviceProvider);
                }
                else
                {
                    using (IScopedServiceProvider localServiceProvider = scopedServiceProviderFactory.GetServiceProvider())
                    {
                        return AddRequestErrorPrivate(requestId, errorDescription, localServiceProvider);
                    }
                }
            }

            return false;
        }

        protected bool AddResponse(FLUXResponseMessageType response, IScopedServiceProvider serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                return AddResponsePrivate(response, serviceProvider);
            }
            else
            {
                using (IScopedServiceProvider localServiceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    return AddResponsePrivate(response, localServiceProvider);
                }
            }
        }

        protected bool AddResponse<TResponse>(FLUXReportDocumentType document, 
                                              TResponse response, 
                                              IScopedServiceProvider serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                return AddResponsePrivate(document, response, serviceProvider);
            }
            else
            {
                using (IScopedServiceProvider localServiceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    return AddResponsePrivate(document, response, localServiceProvider);
                }
            }
        }

        protected bool AddResponse<TResponse>(FLUXResponseDocumentType document, 
                                              TResponse response, 
                                              IScopedServiceProvider serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                return AddResponsePrivate(document, response, serviceProvider);
            }
            else
            {
                using (IScopedServiceProvider localServiceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    return AddResponsePrivate(document, response, localServiceProvider);
                }
            }
        }

        protected void LogException(Exception ex, IScopedServiceProvider serviceProvider = null)
        {
            if (serviceProvider == null)
            {
                using (IScopedServiceProvider localServiceProvider = scopedServiceProviderFactory.GetServiceProvider())
                {
                    var logger = localServiceProvider.GetRequiredService<IExtendedLogger>();
                    logger.LogException(ex);
                }
            }
            else
            {
                var logger = serviceProvider.GetRequiredService<IExtendedLogger>();
                logger.LogException(ex);
            }
        }

        protected TReturn Retry<TReturn>(Func<TReturn> func)
        {
            int count = 0;

            do
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    LogException(ex);

                    count++;
                }
            } while (count < MAX_RETRY_COUNT);

            return default;
        }

        protected Task<bool> SaveResponse(FLUXResponseMessageType response, CancellationToken token)
        {
            return Task.FromResult(AddResponse(response));
        }

        protected Task<bool> SendMessageToFlux<T>(FLUXReportDocumentType document, T message, string domainName, string url)
        {
            IDType id = FindReportDocumentId(document);

            return SendMessageToFlux(id, message, domainName, url);
        }

        protected async Task<bool> SendMessageToFlux<T>(IDType id, T message, string domainName, string url)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            Guid requestId = ParseGUID(id);
            try
            {
                using (ISecureHttpClient secureHttpClient = serviceProvider.GetRequiredService<ISecureHttpClient>())
                {
                    secureHttpClient.BaseAddress = new Uri(ExternalSystemSettings.Default.FluxBaseUrl);
                    AddOrUpdateRequest(requestId, message, true, domainName, url);

                    HttpResponseMessage response = await secureHttpClient.SendAsync(url, message, ensureSuccessStatus: true);

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new HttpRequestException(ErrorResources.msgFluxFailedReceive);
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

        private bool AddOrUpdateRequestPrivate<TRequest>(Guid id, 
                                                         TRequest request, 
                                                         bool isOutgoing, 
                                                         string domainName,
                                                         string webServiceUrl, 
                                                         IScopedServiceProvider serviceProvider)
        {
            try
            {
                IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();

                var dbRequest = db.Fluxfvmsrequests.Where(x => x.RequestUuid == id).FirstOrDefault();
                if (dbRequest == null)
                {
                    string json = TL.SysToSysSecCom.CommonUtils.JsonSerialize(request);
                    db.Fluxfvmsrequests.AddRange(new Fluxfvmsrequest
                    {
                        RequestUuid = id,
                        IsOutgoing = isOutgoing,
                        Attempts = 1,
                        RequestContent = json,
                        DomainName = domainName,
                        WebServiceName = webServiceUrl.Split('/').Last(),
                        RequestDateTime = DateTime.Now
                    });
                }
                else
                {
                    dbRequest.Attempts++;
                }

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);
                return false;
            }
        }

        private bool AddRequestErrorPrivate(Guid requestId, string errorDescription, IScopedServiceProvider serviceProvider)
        {
            try
            {
                IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();
                var dbRequest = db.Fluxfvmsrequests.Where(x => x.RequestUuid == requestId).First();
                dbRequest.ErrorDescription = errorDescription;

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);
                return false;
            }
        }

        private bool AddResponsePrivate<TResponse>(FLUXReportDocumentType document, 
                                                   TResponse response, 
                                                   IScopedServiceProvider serviceProvider)
        {
            try
            {
                IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();
                var logRecord = db.Fluxfvmsrequests.Where(x => x.RequestUuid == Guid.Parse(document.ReferencedID.Value)).First();
                logRecord.ResponseUuid = Guid.Parse(document.ID.Where(x => x.schemeID == IDTypes.UUID).Select(x => x.Value).First());
                logRecord.ResponseDateTime = DateTime.Now;
                logRecord.ResponseContent = TL.SysToSysSecCom.CommonUtils.JsonSerialize(response);
                logRecord.ResponseStatus = "OK";

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);
                return false;
            }
        }

        private bool AddResponsePrivate<TResponse>(FLUXResponseDocumentType document, 
                                                   TResponse response, 
                                                   IScopedServiceProvider serviceProvider)
        {
            try
            {
                IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();
                var logRecord = db.Fluxfvmsrequests.Where(x => x.RequestUuid == Guid.Parse(document.ReferencedID.Value)).First();
                logRecord.ResponseUuid = Guid.Parse(document.ID.Where(x => x.schemeID == IDTypes.UUID).Select(x => x.Value).First());
                logRecord.ResponseDateTime = DateTime.Now;
                logRecord.ResponseContent = TL.SysToSysSecCom.CommonUtils.JsonSerialize(response);
                logRecord.ResponseStatus = "OK";

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);
                return false;
            }
        }

        private bool AddResponsePrivate(FLUXResponseMessageType response, IScopedServiceProvider serviceProvider)
        {
            return AddResponsePrivate(response.FLUXResponseDocument, serviceProvider);
        }

        private bool AddResponsePrivate(FLUXResponseDocumentType response, IScopedServiceProvider serviceProvider)
        {
            try
            {
                IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();
                var logRecord = db.Fluxfvmsrequests.Where(x => x.RequestUuid == Guid.Parse(response.ReferencedID.Value)).First();

                string guid = response.ID.Where(x => x.schemeID == IDTypes.UUID).Select(x => x.Value).FirstOrDefault();

                if (string.IsNullOrEmpty(guid))
                {
                    guid = response.ID.Select(x => x.Value).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(guid) && Guid.TryParse(guid, out Guid result))
                {
                    logRecord.ResponseUuid = result;
                }
                else
                {
                    logRecord.ErrorDescription = "Invalid response UUID";
                    logRecord.ResponseUuid = Guid.Empty;
                }

                logRecord.ResponseDateTime = DateTime.Now;
                logRecord.ResponseContent = TL.SysToSysSecCom.CommonUtils.JsonSerialize(response);

                switch (response.ResponseCode.Value)
                {
                    case nameof(FluxResponseStatuses.OK):
                        {
                            logRecord.ResponseStatus = FluxResponseStatuses.OK.ToString();
                        }
                        break;
                    case "WOK":
                        {
                            logRecord.ResponseStatus = FluxResponseStatuses.Warning.ToString();

                            var errors = response
                                            ?.RelatedValidationResultDocument
                                            ?.SelectMany(x => x?.RelatedValidationQualityAnalysis
                                                              .SelectMany(y => y?.Result)
                                                              .Select(z => z.Value)).ToList();
                            if (errors.Any())
                            {
                                logRecord.ErrorDescription = string.Join(Environment.NewLine, errors);
                            }
                        }
                        break;
                    case "NOK":
                        {
                            logRecord.ResponseStatus = FluxResponseStatuses.Error.ToString();

                            List<string> errors = response
                                                 ?.RelatedValidationResultDocument
                                                 ?.SelectMany(x => x?.RelatedValidationQualityAnalysis
                                                                   .SelectMany(y => y?.Result)
                                                                   .Select(z => z.Value)).ToList();
                            if (errors.Any())
                            {
                                logRecord.ErrorDescription = string.Join(Environment.NewLine, errors);
                            }
                        }
                        break;
                    default:
                        break;
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, serviceProvider);
                return false;
            }
        }

        private static IDType FindReportDocumentId(FLUXReportDocumentType document)
        {
            return document.ReferencedID ?? document.ID.Where(x => x.schemeID == IDTypes.UUID).First();
        }

        private static IDType FindReportDocumentId(FLUXResponseDocumentType document)
        {
            return document.ReferencedID ?? document.ID.Where(x => x.schemeID == IDTypes.UUID).First();
        }

        private Guid ParseGUID(IDType id)
        {
            if (id != null && !string.IsNullOrEmpty(id.Value))
            {
                return Guid.Parse(id.Value);
            }
            else
            {
                return Guid.Empty;
            }
        }
    }
}
