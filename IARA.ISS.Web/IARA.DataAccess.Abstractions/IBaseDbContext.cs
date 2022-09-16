using System;

namespace IARA.DataAccess.Abstractions
{
    public interface IBaseDbContext : IDisposable
    {
        int SaveChanges();
    }
}
