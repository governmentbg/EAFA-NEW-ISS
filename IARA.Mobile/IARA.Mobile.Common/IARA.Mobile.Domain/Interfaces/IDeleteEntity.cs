namespace IARA.Mobile.Domain.Interfaces
{
    public interface IDeleteEntity : IEntity
    {
        bool HasBeenDeletedLocally { get; set; }
    }
}
