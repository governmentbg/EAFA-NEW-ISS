namespace IARA.EntityModels.Interfaces
{
    public interface ILogBookPageEntity
    {
        int Id { get; set; }
        int LogBookId { get; set; }
        string Status { get; set; }
    }

    public interface ILogBookPageDecimalEntity : ILogBookPageEntity
    {
        decimal PageNum { get; set; }
    }

    public interface ILogBookPageStringEntity : ILogBookPageEntity
    {
        string PageNum { get; set; }
    }
}
