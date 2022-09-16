using IARA.EntityModels.Entities;

namespace IARA.EntityModels.Interfaces
{
    public interface IBaseApplicationRegisterEntity : IIdentityRecord
    {
        int? RegisterApplicationId { get; set; }
        string RecordType { get; set; }
    }

    public interface IApplicationRegisterEntity : IApplicationIdentifier, IBaseApplicationRegisterEntity, ISoftDeletable
    {

    }

    public interface IApplicationRegisterValidityEntity : IApplicationIdentifier, IBaseApplicationRegisterEntity, IValidity
    {

    }

    public interface INullableApplicationRegisterEntity : IApplicationNullableIdentifier, IBaseApplicationRegisterEntity, ISoftDeletable
    {

    }

    public interface INullableApplicationRegisterValidityEntity : IApplicationNullableIdentifier, IBaseApplicationRegisterEntity, IValidity
    {

    }

    public interface IChangeOfCircumstancesEntity : IApplicationIdentifier, IIdentityRecord
    {
        int ChangeOfCircumstancesTypeId { get; }

        bool IsActive { get; }
    }

    public interface IApplicationEntity : IApplicationRegisterEntity
    {
        public int? SubmittedForPersonId { get; set; }
        public Person SubmittedForPerson { get; set; }

        public int? SubmittedForLegalId { get; set; }
        public Legal SubmittedForLegal { get; set; }
    }

    public interface IApplicationIdentifier
    {
        int ApplicationId { get; set; }
    }

    public interface IApplicationNullableIdentifier
    {
        int? ApplicationId { get; set; }
    }
}
