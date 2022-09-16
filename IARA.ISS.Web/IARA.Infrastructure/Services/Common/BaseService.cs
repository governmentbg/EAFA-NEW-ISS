using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Exceptions;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure
{
    public abstract class BaseService
    {
        protected IARADbContext Db;

        protected BaseService(IARADbContext dbContext)
        {
            this.Db = dbContext;
        }


        protected T GetActiveRecord<T>(DbSet<T> dbSet, int id)
                where T : class, IIdentityRecord, ISoftDeletable
        {
            try
            {
                return dbSet.Where(x => x.IsActive && x.Id == id).First();
            }
            catch (InvalidOperationException ex)
            {
                throw new RecordNotFoundException("Record not found!", ex);
            }
        }

        protected T GetInactiveRecord<T>(DbSet<T> dbSet, int id)
                where T : class, IIdentityRecord, ISoftDeletable
        {
            try
            {
                return dbSet.Where(x => !x.IsActive && x.Id == id).First();
            }
            catch (InvalidOperationException ex)
            {
                throw new RecordNotFoundException("Record not found!", ex);
            }
        }

        protected T GetActiveValidityRecord<T>(DbSet<T> dbSet, int id)
        where T : class, IIdentityRecord, IValidity
        {
            try
            {
                var now = DateTime.Now;
                return dbSet.Where(x => x.ValidFrom < now
                                && x.ValidTo > now
                                && x.Id == id).First();
            }
            catch (InvalidOperationException ex)
            {
                throw new RecordNotFoundException("Record not found!", ex);
            }
        }

        protected T GetInactiveValidityRecord<T>(DbSet<T> dbSet, int id)
                where T : class, IIdentityRecord, IValidity
        {
            try
            {
                var now = DateTime.Now;
                return dbSet.Where(x => (x.ValidFrom > now || x.ValidTo < now)
                                     && x.Id == id).First();
            }
            catch (InvalidOperationException ex)
            {
                throw new RecordNotFoundException("Record not found!", ex);
            }
        }

        protected T DeleteRecordWithId<T>(DbSet<T> dbSet, int id)
                where T : class, ISoftDeletable, IIdentityRecord
        {
            T record = this.GetActiveRecord(dbSet, id);
            record.IsActive = false;
            return record;
        }

        protected T UndoDeleteRecordWithId<T>(DbSet<T> dbSet, int id)
                where T : class, ISoftDeletable, IIdentityRecord
        {
            T record = this.GetInactiveRecord(dbSet, id);
            record.IsActive = true;
            return record;
        }

        protected T DeleteValidityRecordWithId<T>(DbSet<T> dbSet, int id)
            where T : class, IValidity, IIdentityRecord
        {
            T record = this.GetActiveValidityRecord(dbSet, id);
            record.ValidTo = DateTime.Now.AddSeconds(-1);
            return record;
        }

        protected T UndoDeleteValidityRecordWithId<T>(DbSet<T> dbSet, int id)
         where T : class, IValidity, IIdentityRecord
        {
            T record = this.GetInactiveValidityRecord(dbSet, id);
            record.ValidTo = DefaultConstants.MAX_VALID_DATE;
            return record;
        }

        protected List<NomenclatureDTO> GetNomenclature<T>(DbSet<T> dbSet)
                where T : class, INomenclature, IValidity
        {
            DateTime now = DateTime.Now;

            return dbSet
                .Select(x => new NomenclatureDTO
                {
                    DisplayName = x.Name,
                    Value = x.Id,
                    IsActive = x.ValidFrom < now && x.ValidTo > now
                })
                .OrderBy(x => x.DisplayName)
                .ToList();
        }

        protected List<NomenclatureDTO> GetCodeNomenclature<T>(DbSet<T> dbSet)
                where T : class, INomenclature, ICodeEntity, IValidity
        {
            DateTime now = DateTime.Now;

            return dbSet
                .Select(x => new NomenclatureDTO
                {
                    DisplayName = x.Name,
                    Value = x.Id,
                    Code = x.Code,
                    IsActive = x.ValidFrom < now && x.ValidTo > now
                })
                .OrderBy(x => x.DisplayName)
                .ToList();
        }

        protected DownloadableFileDTO GetFileForDownload<T>(int relatedDocumentId, int fileId)
            where T : class, IFileEntity<T>
        {
            return Db.Set<T>()
                .Where(x => x.Id == relatedDocumentId && x.FileId == fileId)
                .Select(x => new DownloadableFileDTO
                {
                    Bytes = x.File.Content,
                    MimeType = x.File.MimeType,
                    FileName = x.File.Name
                }).Single();
        }


        public void Dispose()
        {
            this.Db.Dispose();
        }
    }
}
