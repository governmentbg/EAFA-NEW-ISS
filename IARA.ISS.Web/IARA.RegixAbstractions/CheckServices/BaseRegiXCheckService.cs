using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Interfaces;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.BatchWorkers.Interfaces;
using TL.RegiXClient;
using TL.RegiXClient.Base;
using TL.RegiXClient.Base.Models;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration
{
    public abstract class BaseRegiXCheckService<TRequest, TResponse, TCompare> : IBaseRegiXCheckService<TRequest, TCompare>
        where TRequest : class
        where TResponse : class
        where TCompare : class
    {
        protected const ushort RETRIES = 3;
        protected static readonly ushort REGIX_CACHE_HOURS = 24;

        protected readonly ConnectionStrings connectionString;
        protected ScopedServiceProviderFactory scopedServiceProviderFactory;
        private readonly IRegixConclusionsService regixConclusionsService;
        protected readonly IAsyncWorkerTaskQueue<RegixContextData<TRequest, TCompare>, TResponse> workerQueue;
        private readonly string queueName;
        protected readonly string operationName;
        private IRegiXClientService regixService;
        private bool disposedValue;
        private readonly bool shouldCheckCache = true;
        private readonly bool saveMappedResponse = false;


        protected EmployeeInfo GetEmployeeInfo(BaseContextData data)
        {
            var employee = new EmployeeInfo
            {
                EmployeeEGN = !string.IsNullOrEmpty(data.EGN) ? data.EGN : "System",
                EmployeeNames = !string.IsNullOrEmpty(data.EmployeeNames) ? data.EmployeeNames : "IARA",
                EmployeeIdentifier = !string.IsNullOrEmpty(data.EmployeeIdentifier) ? data.EmployeeIdentifier : "System",
            };

            return employee;
        }

        protected RequestParameters GetRequestParameters(BaseContextData data)
        {
            return new RequestParameters
            {
                LawReason = $"{data.ServiceType}  {data.ServiceURI}",
                ServiceURI = data.ServiceURI,
                ServiceType = data.ServiceType,
            };
        }

        protected BaseRegiXCheckService(ConnectionStrings connectionString,
                                                IRegiXClientService regixService,
                                                IRegixConclusionsService regixConclusionsService,
                                                ScopedServiceProviderFactory scopedServiceProviderFactory,
                                                string queueName,
                                                string operationName,
                                                bool shouldCheckCache = true,
                                                bool saveMappedResponse = false)
        {
            this.saveMappedResponse = saveMappedResponse;
            this.shouldCheckCache = shouldCheckCache;
            this.queueName = queueName;
            this.operationName = operationName;
            this.connectionString = connectionString;
            this.regixService = regixService;
            this.regixConclusionsService = regixConclusionsService;
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
            this.workerQueue = WorkerCreationUtils.CreateWorkerQueue<RegixContextData<TRequest, TCompare>, TResponse>(HandleDataResponsePrv, connectionString, queueName);
            //this.workerQueue.TaskCompleted += this.WorkerQueue_TaskCompleted;
            //this.workerQueue.TaskErrorOccured += this.WorkerQueue_TaskErrorOccured;
        }

        public Task<bool> Enqueue(RegixContextData<TRequest, TCompare> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return workerQueue.Enqueue(request, priority, timeToDelay);
        }

        public void AddRegixCheck(RegixContextData<TRequest, TCompare> request, IScopedServiceProvider scopedServiceProvider)
        {
            request.CompareWithObject = GetLocalData(request);
            request.Db = scopedServiceProvider.GetRequiredService<IARADbContext>();
            AddOrUpdateCheckRequest(request, request.CompareWithObject, scopedServiceProvider, 0);

            request.Db = null;
        }

        protected async Task<bool> TryUpdateApplicationStatus(int applicationId, int applicationHistoryId)
        {
            bool result = await regixConclusionsService.FinalDecision(applicationId, applicationHistoryId);

            using (IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider())
            {
                IExtendedLogger logger = serviceProvider.GetService<IExtendedLogger>();

                if (result)
                {
                    logger.LogInfo($"Решение за: ApplicationID: {applicationId} ApplicationHistoryID: {applicationHistoryId}");
                }
                else
                {
                    logger.LogWarning($"Неуспешно добавено към опашка: {queueName}. Решение за: ApplicationID: {applicationId} ApplicationHistoryID: {applicationHistoryId}");
                }
            }
            return result;
        }

        protected abstract RegixCheckStatus CompareApplicationData(TCompare response, TCompare compare, BaseContextData context);
        protected virtual TCompare GetLocalData(RegixContextData<TRequest, TCompare> request)
        {
            return request.CompareWithObject;
        }

        protected abstract TCompare MapToLocalData(TResponse response, RegixContextData<TRequest, TCompare> request);

        protected abstract Task<ResponseType<TResponse>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<TRequest, TCompare> request);

        protected void LogException(Exception ex, IScopedServiceProvider scopedServiceProvider)
        {
            IExtendedLogger logger = scopedServiceProvider.GetRequiredService<IExtendedLogger>();
            logger.LogException(ex);
        }

        protected int AddResponseToCache(string operationName, string identifier, ContextCheckType identifierType, TRequest request, TResponse response, IARADbContext db)
        {
            ApplicationRegiXcache cache = new ApplicationRegiXcache
            {
                RequestContent = CommonUtils.Serialize(request),
                ResponseContent = response != null ? CommonUtils.Serialize(response) : default,
                ResponseDateTime = DateTime.Now,
                WebServiceName = operationName
            };

            switch (identifierType)
            {
                case ContextCheckType.AgentPerson:
                case ContextCheckType.ChangePerson:
                case ContextCheckType.LessorPerson:
                case ContextCheckType.HolderPerson:
                case ContextCheckType.OrganizerPerson:
                case ContextCheckType.OwnerPerson:
                case ContextCheckType.ReceiverPerson:
                case ContextCheckType.RepresentativePerson:
                case ContextCheckType.RequesterPerson:
                case ContextCheckType.SubmittedByPerson:
                case ContextCheckType.LegalAuthorizedPerson:
                case ContextCheckType.SubmittedForPerson:
                case ContextCheckType.DisabledPerson:
                    cache.EgnLnc = identifier;
                    break;
                case ContextCheckType.ChangeLegal:
                case ContextCheckType.HolderLegal:
                case ContextCheckType.Legal:
                case ContextCheckType.LessorLegal:
                case ContextCheckType.OwnerLegal:
                case ContextCheckType.SubmittedForLegal:
                    cache.Eik = identifier;
                    break;
                case ContextCheckType.AgentAddress:
                case ContextCheckType.ChangePersonAddress:
                case ContextCheckType.ChangeLegalAddress:
                case ContextCheckType.HolderPersonAddress:
                case ContextCheckType.HolderLegalAddress:
                case ContextCheckType.LessorPersonAddress:
                case ContextCheckType.LessorLegalAddress:
                case ContextCheckType.OrganizerAddress:
                case ContextCheckType.OwnerPersonAddress:
                case ContextCheckType.OwnerLegalAddress:
                case ContextCheckType.RepresentativeAddress:
                case ContextCheckType.RequesterAddress:
                case ContextCheckType.SubmittedByAddress:
                case ContextCheckType.SubmittedForPersonAddress:
                case ContextCheckType.SubmittedForLegalAddress:
                case ContextCheckType.ReceiverAddress:
                    cache.EgnLnc = identifier;
                    break;
            }

            db.ApplicationRegiXcaches.Add(cache);

            return db.SaveChanges();
        }

        protected bool TryGetFromCache(string operationName, string identifier, ContextCheckType identifierType, IARADbContext db, out TResponse response)
        {
            bool found = false;
            response = null;

            switch (identifierType)
            {
                case ContextCheckType.AgentPerson:
                case ContextCheckType.ChangePerson:
                case ContextCheckType.LessorPerson:
                case ContextCheckType.HolderPerson:
                case ContextCheckType.OrganizerPerson:
                case ContextCheckType.OwnerPerson:
                case ContextCheckType.ReceiverPerson:
                case ContextCheckType.RequesterPerson:
                case ContextCheckType.SubmittedByPerson:
                case ContextCheckType.LegalAuthorizedPerson:
                case ContextCheckType.SubmittedForPerson:
                case ContextCheckType.DisabledPerson:
                    {
                        string result = (from x in db.ApplicationRegiXcaches
                                         where x.WebServiceName == operationName
                                         && (DateTime.Now - x.ResponseDateTime) < TimeSpan.FromHours(REGIX_CACHE_HOURS)
                                         && x.EgnLnc == identifier
                                         select x.ResponseContent).FirstOrDefault();

                        if (result != null)
                        {
                            response = CommonUtils.Deserialize<TResponse>(result);
                            found = true;
                        }
                    }
                    break;
                case ContextCheckType.ChangeLegal:
                case ContextCheckType.HolderLegal:
                case ContextCheckType.Legal:
                case ContextCheckType.LessorLegal:
                case ContextCheckType.OwnerLegal:
                case ContextCheckType.SubmittedForLegal:
                    {
                        string result = (from x in db.ApplicationRegiXcaches
                                         where x.WebServiceName == operationName
                                         && (DateTime.Now - x.ResponseDateTime) < TimeSpan.FromHours(REGIX_CACHE_HOURS)
                                         && x.Eik == identifier
                                         select x.ResponseContent).FirstOrDefault();

                        if (result != null)
                        {
                            response = CommonUtils.Deserialize<TResponse>(result);
                            found = true;
                        }
                    }
                    break;
                case ContextCheckType.AgentAddress:
                case ContextCheckType.ChangePersonAddress:
                case ContextCheckType.ChangeLegalAddress:
                case ContextCheckType.HolderPersonAddress:
                case ContextCheckType.HolderLegalAddress:
                case ContextCheckType.LessorPersonAddress:
                case ContextCheckType.LessorLegalAddress:
                case ContextCheckType.OrganizerAddress:
                case ContextCheckType.OwnerPersonAddress:
                case ContextCheckType.OwnerLegalAddress:
                case ContextCheckType.RepresentativeAddress:
                case ContextCheckType.RequesterAddress:
                case ContextCheckType.SubmittedByAddress:
                case ContextCheckType.SubmittedForPersonAddress:
                case ContextCheckType.SubmittedForLegalAddress:
                case ContextCheckType.ReceiverAddress:
                    {
                        string result = (from x in db.ApplicationRegiXcaches
                                         where x.WebServiceName == operationName
                                         && (DateTime.Now - x.ResponseDateTime) < TimeSpan.FromHours(REGIX_CACHE_HOURS)
                                         && x.EgnLnc == identifier
                                         select x.ResponseContent).FirstOrDefault();

                        if (result != null)
                        {
                            response = CommonUtils.Deserialize<TResponse>(result);
                            found = true;
                        }
                    }
                    break;
            }

            return found;
        }

        protected async Task<ResponseType<TResponse>> GetResponse(RegixContextData<TRequest, TCompare> data, Func<TRequest, Task<TResponse>> executeOperation)
        {
            TResponse response;

            if (shouldCheckCache && TryGetFromCache(operationName, data.AdditionalIdentifier, data.Type, data.Db, out response))
            {
                return new ResponseType<TResponse>
                {
                    Response = response,
                    Type = RegixResponseStatusEnum.Cache
                };
            }
            else
            {
                response = await executeOperation(data.Context);

                if (shouldCheckCache)
                {
                    AddResponseToCache(operationName, data.AdditionalIdentifier, data.Type, data.Context, response, data.Db);
                }

                return new ResponseType<TResponse>
                {
                    Response = response,
                    Type = RegixResponseStatusEnum.OK
                };
            }
        }

        private int AddOrUpdateCheckRequest(RegixContextData<TRequest, TCompare> request, TCompare model, IScopedServiceProvider scopedServiceProvider, short attempt = 0)
        {
            ApplicationRegiXcheck applicationCheck = null;

            try
            {
                int checkType = (int)request.Type;

                applicationCheck = (from check in request.Db.ApplicationRegiXchecks
                                    where check.ApplicationId == request.ApplicationId
                                    && check.ApplicationChangeHistoryId == request.ApplicationHistoryId
                                    && check.AdditianalIdentifier == request.AdditionalIdentifier
                                    && check.CheckType == checkType
                                    select check).FirstOrDefault();


                if (applicationCheck == null)
                {
                    applicationCheck = new ApplicationRegiXcheck
                    {
                        ApplicationId = request.ApplicationId,
                        ApplicationChangeHistoryId = request.ApplicationHistoryId,
                        WebServiceName = operationName,
                        AdditianalIdentifier = request.AdditionalIdentifier,
                        CheckType = (int)request.Type,
                        RemoteAddress = regixService.EndpointAddress,
                        RequestDateTime = DateTime.Now,
                        RequestContent = CommonUtils.Serialize(request.Context),
                        ResponseStatus = RegixResponseStatusEnum.NoResponse.ToString(),
                        ErrorLevel = RegixCheckStatusesEnum.NONE.ToString(),
                        ExpectedResponseContent = CommonUtils.Serialize(model, model.GetType()),
                        Attempts = attempt
                    };

                    applicationCheck = request.Db.ApplicationRegiXchecks.Add(applicationCheck).Entity;
                }
                else
                {
                    applicationCheck.Attempts++;
                }

                request.Db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogException(ex, scopedServiceProvider);
            }

            return applicationCheck.Id;
        }

        private async Task<TResponse> HandleDataResponsePrv(RegixContextData<TRequest, TCompare> requestContext, CancellationToken token)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            requestContext.Db = scopedServiceProvider.GetRequiredService<IARADbContext>();
            requestContext.ServiceProvider = scopedServiceProvider;

            requestContext.CompareWithObject = GetLocalData(requestContext);
            int checkId = AddOrUpdateCheckRequest(requestContext, requestContext.CompareWithObject, scopedServiceProvider, 1);

            ResponseType<TResponse> result = null;

            try
            {
                result = await HandleDataResponse(regixService, requestContext);

                if (result != null)
                {
                    TCompare mappedResult = MapToLocalData(result.Response, requestContext);

                    RegixCheckStatus checkStatus = CompareApplicationData(requestContext.CompareWithObject, mappedResult, requestContext);
                    object resultToBeSaved;

                    if (saveMappedResponse)
                    {
                        resultToBeSaved = mappedResult;
                    }
                    else
                    {
                        resultToBeSaved = result.Response;
                    }

                    UpdateRegiXResponse(checkId, resultToBeSaved, result.Type, checkStatus.Status, checkStatus.ErrorDescription, requestContext.Db);
                    await TryUpdateApplicationStatus(requestContext.ApplicationId, requestContext.ApplicationHistoryId);
                }
                else
                {
                    UpdateRegiXResponse(checkId, result, result.Type, RegixCheckStatusesEnum.ERROR, ErrorResources.msgEmptyRegixDataReceived, requestContext.Db);
                    await TryUpdateApplicationStatus(requestContext.ApplicationId, requestContext.ApplicationHistoryId);
                }
            }
            catch (RegiXException ex)
            {
                UpdateRegiXResponse(checkId, null, RegixResponseStatusEnum.Error, RegixCheckStatusesEnum.ERROR, ex.Message, requestContext.Db);
                LogException(ex, scopedServiceProvider);
                return null;
            }
            catch (Exception ex)
            {
                int attempts = UpdateRegiXResponse(checkId, null, RegixResponseStatusEnum.Error, RegixCheckStatusesEnum.ERROR, ex.Message, requestContext.Db);
                LogException(ex, scopedServiceProvider);

                if (attempts < RETRIES)
                {
                    await workerQueue.Enqueue(requestContext);
                }
            }

            return result.Response;
        }

        protected virtual int UpdateRegiXResponse(int checkId, object response, RegixResponseStatusEnum responseStatus, RegixCheckStatusesEnum errorLevel, string errorMessage, IARADbContext db)
        {
            ApplicationRegiXcheck applicationCheck = (from check in db.ApplicationRegiXchecks
                                                      where check.Id == checkId
                                                      select check).First();

            applicationCheck.ErrorLevel = errorLevel.ToString();
            applicationCheck.ResponseDateTime = DateTime.Now;
            applicationCheck.ResponseContent = response != null ? CommonUtils.Serialize(response, response.GetType()) : null;
            applicationCheck.ErrorDescription = errorMessage;
            applicationCheck.ResponseStatus = responseStatus.ToString();

            db.SaveChanges();

            return applicationCheck.Attempts;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    workerQueue?.Dispose();
                    scopedServiceProviderFactory = null;
                    regixService = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
