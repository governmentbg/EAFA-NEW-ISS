using System.Collections.Generic;
using SQLite;

namespace IARA.Mobile.Domain.Models
{
    public class TLTableQuery<TEntity> : TableQuery<TEntity>
    {
        public TLTableQuery(SQLiteConnection conn)
            : base(conn) { }

        public int Add(TEntity item)
        {
            return Connection.Insert(item);
        }

        public int AddRange(IEnumerable<TEntity> collection)
        {
            return Connection.InsertAll(collection);
        }

        public int Update(TEntity item)
        {
            return Connection.Update(item);
        }

        public int UpdateRange(IEnumerable<TEntity> collection)
        {
            return Connection.UpdateAll(collection);
        }

        public int Remove(TEntity item)
        {
            return Connection.Delete(item);
        }

        public int Remove(int primaryKey)
        {
            return Connection.Delete<TEntity>(primaryKey);
        }

        public int RemoveRange(IEnumerable<TEntity> collection)
        {
            return Connection.UpdateAll(collection);
        }

        public int Clear()
        {
            return Connection.DeleteAll<TEntity>();
        }
    }
}
