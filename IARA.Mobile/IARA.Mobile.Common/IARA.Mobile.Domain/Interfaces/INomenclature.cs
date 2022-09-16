namespace IARA.Mobile.Domain.Interfaces
{
    public interface INomenclature : IEntity
    {
        string Name { get; set; }
        bool IsActive { get; set; }
    }
}
