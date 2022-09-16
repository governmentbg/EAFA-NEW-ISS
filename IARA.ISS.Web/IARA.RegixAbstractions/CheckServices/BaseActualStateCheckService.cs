using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.RegixAbstractions.CheckServices;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.ActualState;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BaseActualStateCheckService : BaseRegiXCheckService<ActualStateRequestType, ActualStateResponseType, RegixLegalContext>, IActualStateCheckService
    {
        protected BaseActualStateCheckService(ConnectionStrings connectionString,
                                              IRegiXClientService regixService,
                                              IRegixConclusionsService regixConclusionsService,
                                              ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.GetActualState),
                   Operations.GET_ACTUAL_STATE)
        { }

        protected abstract override Task<ResponseType<ActualStateResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<ActualStateRequestType, RegixLegalContext> data);

        protected override RegixCheckStatus CompareApplicationData(RegixLegalContext response, RegixLegalContext compare, BaseContextData context)
        {
            var registrationAddress = response.Addresses?.Where(x => x.AddressType == AddressTypesEnum.COURT_REGISTRATION).FirstOrDefault();
            var registrationAddressOther = compare.Addresses?.Where(x => x.AddressType == AddressTypesEnum.COURT_REGISTRATION).FirstOrDefault();
            var correspondenceAddress = response.Addresses?.Where(x => x.AddressType == AddressTypesEnum.CORRESPONDENCE).FirstOrDefault();
            var correspondenceAddressOther = compare.Addresses?.Where(x => x.AddressType == AddressTypesEnum.CORRESPONDENCE).FirstOrDefault();

            RegixCheckStatus registrationAddressStatus = RegixCheckUtils.GetAddressCheckStatus(registrationAddress, registrationAddressOther);
            RegixCheckStatus correspondenceAddressStatus = RegixCheckUtils.GetAddressCheckStatus(correspondenceAddress, correspondenceAddressOther);

            if (!RegixCheckUtils.Equals(response.Legal, response.Legal, out List<string> errorProperties, true, x => x.Name))
            {
                string errorDescription = string.Format(ErrorResources.msgPropertiesDiscrepancy, string.Join(',', errorProperties));

                if (RegixCheckUtils.Equals(response.Legal, response.Legal, out errorProperties, false, x => x.Name))
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
                }
                else
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.ERROR, errorDescription);
                }
            }
            else
            {
                RegixCheckStatus legalCheckStatus;
                if (!RegixCheckUtils.Equals(response.Legal, response.Legal, out List<string> discrepantProperties, true, x => x.Phone, x => x.Email))
                {
                    string errorDescription = string.Format(ErrorResources.msgPropertiesDiscrepancy, string.Join(',', errorProperties));
                    legalCheckStatus = new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
                }
                else
                {
                    legalCheckStatus = new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
                }

                if (legalCheckStatus.Status == RegixCheckStatusesEnum.WARN
                    || registrationAddressStatus.Status == RegixCheckStatusesEnum.WARN
                    || correspondenceAddressStatus.Status == RegixCheckStatusesEnum.WARN)
                {
                    string errorDescription = RegixCheckUtils.GetCommonErrorDescription(legalCheckStatus, registrationAddressStatus, correspondenceAddressStatus);

                    return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
                }
                else
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
                }
            }
        }

        protected override RegixLegalContext MapToLocalData(ActualStateResponseType response, RegixContextData<ActualStateRequestType, RegixLegalContext> request)
        {
            return RegixDataMappers.MapActualStateResponse(response);
        }
    }
}
