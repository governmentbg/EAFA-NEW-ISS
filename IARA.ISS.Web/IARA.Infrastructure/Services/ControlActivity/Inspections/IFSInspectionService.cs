using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class IFSInspectionService : BaseInspectionService<InspectionFirstSaleDTO>
    {
        public IFSInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionFirstSaleDTO Get(InspectionFirstSaleDTO inspection)
        {
            InspectionFirstSaleDTO ifsDTO = (
                from insp in Db.FirstSaleInspections
                where insp.InspectionId == inspection.Id.Value
                select new InspectionFirstSaleDTO
                {
                    SubjectName = insp.Name,
                    SubjectAddress = insp.Address,
                    RepresentativeComment = insp.RepresentativeComment,
                }).Single();

            ifsDTO.CatchMeasures = GetDeclarationCatchMeasures(inspection.Id.Value);

            return AssignFromBase(ifsDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionFirstSaleDTO ifsDTO)
        {
            FirstSaleInspection newIfsDbEntry = new()
            {
                Inspection = inspDbEntry,
                Address = ifsDTO.SubjectAddress,
                Name = ifsDTO.SubjectName,
                RepresentativeComment = ifsDTO.RepresentativeComment,
                IsActive = true
            };

            AddDeclarationCatchMeasures(inspDbEntry, ifsDTO.CatchMeasures);

            Db.FirstSaleInspections.Add(newIfsDbEntry);
        }
    }
}
