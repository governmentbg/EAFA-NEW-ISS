using IARA.EntityModels.Entities;

namespace IARA.EntityModels.Interfaces
{
    public interface IFileEntity
    {
        int Id { get; set; }

        int RecordId { get; set; }

        int FileId { get; set; }

        int FileTypeId { get; set; }

        File File { get; set; }
    }

    public interface IFileEntity<T> : IFileEntity
    {
        T Record { get; set; }
    }
}
