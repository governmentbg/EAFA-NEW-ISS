using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Interfaces.FluxIntegrations.FishingDiaries;

namespace IARA.Infrastructure.FluxIntegrations.Mappings
{
    public class FishingActivitiesDomainMapper : BaseService, IFishingActivitiesDomainMapper
    {
        public FishingActivitiesDomainMapper(IARADbContext dbContext)
            : base(dbContext)
        { }

        public FLUXFAQueryMessageType MapQuery(FluxFAQueryRequestEditDTO request)
        {
            FLUXFAQueryMessageType result = new()
            {
                FAQuery = new FAQueryType
                {
                    ID = IDType.CreateFromGuid(Guid.NewGuid()),
                    SubmittedDateTime = DateTime.Now,
                    TypeCode = CodeType.CreateCode(ListIDTypes.FA_QUERY_TYPE, request.QueryType.ToString()),
                    SubmitterFLUXParty = new FLUXPartyType
                    {
                        ID = new IDType[] { IDType.CreateParty("BGR") },
                        Name = new TextType[] { TextType.CreateText("Bulgaria") }
                    },
                    SpecifiedDelimitedPeriod = request.QueryType == FluxFAQueryTypesEnum.VESSEL
                        ? new DelimitedPeriodType
                        {
                            StartDateTime = request.DateTimeFrom.Value,
                            EndDateTime = request.DateTimeTo.Value
                        }
                        : null,
                    SimpleFAQueryParameter = CreateFAQueryParameters(request)
                }
            };

            return result;
        }

        private static FAQueryParameterType[] CreateFAQueryParameters(FluxFAQueryRequestEditDTO request)
        {
            List<FAQueryParameterType> result = new();

            if (request.QueryType == FluxFAQueryTypesEnum.VESSEL)
            {
                if (!string.IsNullOrEmpty(request.VesselCFR))
                {
                    result.Add(new FAQueryParameterType
                    {
                        TypeCode = CodeType.CreateCode(ListIDTypes.FA_QUERY_PARAMETER_TYPE, nameof(FaQueryParameterTypeCodes.VESSELID)),
                        ValueID = IDType.CreateCFR(request.VesselCFR)
                    });
                }
                else if (!string.IsNullOrEmpty(request.VesselIRCS))
                {
                    result.Add(new FAQueryParameterType
                    {
                        TypeCode = CodeType.CreateCode(ListIDTypes.FA_QUERY_PARAMETER_TYPE, nameof(FaQueryParameterTypeCodes.VESSELID)),
                        ValueID = IDType.CreateID(IDTypes.IRCS, request.VesselIRCS)
                    });
                }
                else
                {
                    throw new ArgumentException("No CFR or IRCS provided for FA query!");
                }
            }
            else if (request.QueryType == FluxFAQueryTypesEnum.TRIP)
            {
                result.Add(new FAQueryParameterType
                {
                    TypeCode = CodeType.CreateCode(ListIDTypes.FA_QUERY_PARAMETER_TYPE, nameof(FaQueryParameterTypeCodes.TRIPID)),
                    ValueCode = CodeType.CreateCode(ListIDTypes.BOOLEAN_TYPE, request.Consolidated.Value ? "Y" : "N")
                });
            }

            result.Add(new FAQueryParameterType
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FA_QUERY_PARAMETER_TYPE, nameof(FaQueryParameterTypeCodes.CONSOLIDATED)),
                ValueCode = CodeType.CreateCode(ListIDTypes.BOOLEAN_TYPE, request.Consolidated.Value ? "Y" : "N")
            });

            return result.ToArray();
        }
    }
}
