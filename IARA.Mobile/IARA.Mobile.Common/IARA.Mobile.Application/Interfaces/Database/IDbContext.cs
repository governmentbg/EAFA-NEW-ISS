using System;
using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Application.Interfaces.Database
{
    public interface IDbContext : IDisposable
    {
        TLTableQuery<ErrorLog> ErrorLogs { get; }

        TLTableQuery<TEntity> TLTable<TEntity>(string propertyName);

        TLTableQuery<TEntity> TLTable<TEntity>();

        /// <summary>
        /// Removes all entities from all tables
        /// </summary>
        void DeleteAllTables();
    }
}
