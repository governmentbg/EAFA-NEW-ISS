using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.IAMA;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class VesselCheckService : BaseVesselCheckService
    {
        public VesselCheckService(ConnectionStrings connectionString,
                                  IRegiXClientService regixService,
                                  IRegixConclusionsService regixConclusionsService,
                                  ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override RegixCheckStatus CompareApplicationData(VesselContext response, VesselContext compare, BaseContextData context)
        {
            if (response == null || response.VesselData == null || compare == null || compare.VesselData == null)
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, ErrorResources.msgVesselNotFoundInRegister);
            }
            else
            {
                bool success = RegixCheckUtils.Equals(response, compare, out List<string> errors, true,
                     x => x.VesselData.ShipDraught,
                     x => x.VesselData.Name,
                     x => x.VesselData.GrossTonnage,
                     x => x.VesselData.NetTonnage,
                     x => x.VesselData.RegLicencePublishPage,
                     x => x.VesselData.TotalLength,
                     x => x.VesselData.TotalWidth,
                     x => x.VesselData.RegLicencePublishVolume);

                int count = response.Owners.Select(x => x.EgnLncEik).ToList().Intersect(compare.Owners.Select(x => x.EgnLncEik).ToList()).Count();

                if (response.Owners.Count != count && compare.Owners.Count != count)
                {
                    errors.Add(ErrorResources.msgMissingOwners);
                    success = false;
                }

                if (success)
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
                }
                else
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, string.Join(',', errors));
                }
            }
        }

        protected override async Task<ResponseType<ShipsResponse>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<VesselRequest, VesselContext> data)
        {
            if (!data.Context.SearchByOwners)
            {
                var vesselRequest = new RegistrationInfoByCharacteristicsRequestType();

                if (!data.Context.MainEngineNumberChanged && !string.IsNullOrEmpty(data.Context.MainEngineNumber))
                {
                    vesselRequest.EngineNumber = data.Context.MainEngineNumber;
                }

                if (!data.Context.VesselTypeChanged && data.Context.VesselType.HasValue)
                {
                    vesselRequest.VesselType = data.Context.VesselType.Value;
                    vesselRequest.VesselTypeSpecified = true;
                }

                if (!data.Context.HullMaterialTypeChanged && !string.IsNullOrEmpty(data.Context.HullNumber))
                {
                    vesselRequest.HullNumber = data.Context.HullNumber;
                }

                if (!data.Context.TotalLengthChanged && data.Context.TotalLength.HasValue && data.Context.TotalLength > 0)
                {
                    vesselRequest.MaxLength = new MaximumLengthType
                    {
                        From = (decimal)Math.Floor(data.Context.TotalLength.Value),
                        FromSpecified = true,
                        To = (decimal)Math.Ceiling(data.Context.TotalLength.Value),
                        ToSpecified = true
                    };

                    if (!data.Context.LengthBetweenPerpendicularsChanged && data.Context.LengthBetweenPerpendiculars.HasValue && data.Context.LengthBetweenPerpendiculars > 0)
                    {
                        vesselRequest.MaxLength.From = Math.Min(vesselRequest.MaxLength.From, (decimal)Math.Floor(data.Context.LengthBetweenPerpendiculars.Value));
                        vesselRequest.MaxLength.To = Math.Max(vesselRequest.MaxLength.To, (decimal)Math.Ceiling(data.Context.LengthBetweenPerpendiculars.Value));
                    }
                }

                var response = await regixClientService.SearchVesselByCharacteristics(vesselRequest, GetRequestParameters(data), GetEmployeeInfo(data));

                return new ResponseType<ShipsResponse>
                {
                    Response = response,
                    Type = RegixResponseStatusEnum.OK
                };
            }
            else
            {
                if (data.Context.OwnersBulstatEGN != null && data.Context.OwnersBulstatEGN.Any())
                {
                    var request = new RegistrationInfoByOwnerRequestType();
                    request.Identifier = data.Context.OwnersBulstatEGN.First();

                    var response = await regixClientService.SearchVesselByOwner(request, GetRequestParameters(data), GetEmployeeInfo(data));

                    return new ResponseType<ShipsResponse>
                    {
                        Response = response,
                        Type = RegixResponseStatusEnum.OK
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        protected override VesselContext MapToLocalData(ShipsResponse response, RegixContextData<VesselRequest, VesselContext> requestContext)
        {
            var request = requestContext.Context;

            if (response != null && response.Ships != null && response.Ships.Length > 0)
            {
                if (response.Ships.Length == 1)
                {
                    return RegixDataMappers.MapRegixVessel(response.Ships[0]);
                }
                else if (request.SearchByOwners)
                {
                    var ship = response.Ships.Select(x => new
                    {
                        Count = x.OwnersInfo.Select(x => x.BulstatEGN).Intersect(request.OwnersBulstatEGN).Count(),
                        Ship = x
                    }).OrderByDescending(x => x.Count).Select(x => x.Ship).FirstOrDefault();

                    if (ship != null)
                    {
                        return RegixDataMappers.MapRegixVessel(ship);
                    }
                }
                else
                {
                    var ships = response.Ships.AsQueryable();

                    if (!request.RegistrationVolumeChanged && !string.IsNullOrEmpty(request.RegistrationVolume))
                    {
                        ships = ships.Where(x => x.RegistrationInfo.Tom == request.RegistrationVolume);
                    }

                    if (!request.RegistrationPageChanged && !string.IsNullOrEmpty(request.RegistrationPage))
                    {
                        ships = ships.Where(x => x.RegistrationInfo.Page == request.RegistrationPage);
                    }

                    var ship = ships.FirstOrDefault();

                    if (ship != null)
                    {
                        return RegixDataMappers.MapRegixVessel(ship);
                    }
                    else
                    {
                        using IScopedServiceProvider serviceProvider = this.scopedServiceProviderFactory.GetServiceProvider();
                        AddRegixCheck(requestContext, serviceProvider);
                        requestContext.Context.SearchByOwners = true;
                        Enqueue(requestContext);

                        return null;
                    }
                }
            }

            return null;
        }
    }
}
