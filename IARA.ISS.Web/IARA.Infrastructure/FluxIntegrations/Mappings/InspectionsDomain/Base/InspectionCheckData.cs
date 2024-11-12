namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain.Base
{
    public class InspectionCheckData
    {
        public string FluxCode { get; set; }

        public string Description { get; set; }

        public string UnregisteredObjectIdentifier { get; set; }

        public InspectionToggleTypesEnum? CheckValue { get; set; }

        public InspectionCheckTypesEnum? CheckType { get; set; }

        public bool HasDescription { get; set; }

        public static List<InspectionCheckData> GetInspectionChecks(int inspectionId, IARADbContext db)
        {
            List<InspectionCheckData> result = (from check in db.InspectionChecks
                                                join checkType in db.NinspectionCheckTypes on check.CheckTypeId equals checkType.Id
                                                where check.InspectionId == inspectionId
                                                   && check.IsActive
                                                   && !string.IsNullOrEmpty(checkType.FLUXCode)
                                                select new InspectionCheckData
                                                {
                                                    FluxCode = checkType.FLUXCode,
                                                    Description = check.Description,
                                                    HasDescription = checkType.HasDescription,
                                                    UnregisteredObjectIdentifier = check.UnregisteredObjectIdentifier,
                                                    CheckType = Enum.Parse<InspectionCheckTypesEnum>(checkType.CheckType.ToUpper()),
                                                    CheckValue = !string.IsNullOrEmpty(check.CheckValue)
                                                                 ? Enum.Parse<InspectionToggleTypesEnum>(check.CheckValue)
                                                                 : null
                                                }).ToList();

            return result;
        }
    }
}
