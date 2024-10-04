namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    public class InspectorData
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public short? SequenceNumber { get; set; }

        public bool IsInCharge { get; set; }

        public static List<InspectorData> GetInspectors(int inspectionId, IARADbContext db)
        {
            List<InspectorData> inspectors = (from inspInspector in db.InspectionInspectors
                                              join inspector in db.Inspectors on inspInspector.InspectorId equals inspector.Id
                                              join user in db.Users on inspector.UserId equals user.Id into userMatchTable
                                              from userMatch in userMatchTable.DefaultIfEmpty()
                                              join person in db.Persons on userMatch.PersonId equals person.Id into personMatchTable
                                              from personMatch in personMatchTable.DefaultIfEmpty()
                                              join unregisteredPerson in db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregisteredPerson.Id into unregisteredPersonMatchTable
                                              from unregisteredPersonMatch in unregisteredPersonMatchTable.DefaultIfEmpty()
                                              where inspInspector.InspectionId == inspectionId
                                                  && inspInspector.IsActive
                                              orderby inspInspector.OrderNum
                                              select new InspectorData
                                              {
                                                  FirstName = personMatch != null ? personMatch.FirstName : unregisteredPersonMatch.FirstName,
                                                  MiddleName = personMatch != null ? personMatch.MiddleName : unregisteredPersonMatch.MiddleName,
                                                  LastName = personMatch != null ? personMatch.LastName : unregisteredPersonMatch.LastName,
                                                  SequenceNumber = inspInspector.OrderNum,
                                                  IsInCharge = inspInspector.IsInCharge
                                              }).ToList();

            return inspectors;
        }
    }
}
