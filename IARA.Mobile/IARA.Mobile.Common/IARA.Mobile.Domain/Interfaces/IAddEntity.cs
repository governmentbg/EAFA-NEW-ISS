namespace IARA.Mobile.Domain.Interfaces
{
    public interface IAddEntity : IEntity
    {
        bool IsLocalOnly { get; set; }
    }
}
