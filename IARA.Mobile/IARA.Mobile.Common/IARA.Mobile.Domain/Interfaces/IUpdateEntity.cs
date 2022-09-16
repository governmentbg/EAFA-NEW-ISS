namespace IARA.Mobile.Domain.Interfaces
{
    public interface IUpdateEntity : IEntity
    {
        bool HasBeenUpdatedLocally { get; set; }
    }
}
