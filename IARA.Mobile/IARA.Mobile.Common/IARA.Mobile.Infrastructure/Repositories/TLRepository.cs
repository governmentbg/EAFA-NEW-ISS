using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Database;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Repositories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Infrastructure.Repositories
{
    public class TLRepository : ITLRepository
    {
        private class UpdateClass<TEntity>
            where TEntity : IOfflineEntity, new()
        {
            public TEntity Entity { get; set; }
            public bool Found { get; set; }
        }

        private readonly IDbContextBuilder contextBuilder;
        private readonly IRestClient restClient;

        public TLRepository(IDbContextBuilder contextBuilder, IRestClient restClient)
        {
            this.contextBuilder = contextBuilder ?? throw new ArgumentNullException(nameof(contextBuilder));
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        public async Task<AddEntityResult> Add<TEntity, TDto>(TEntity entity, TDto dto, string url, object parameters = null, bool asFormData = false)
            where TEntity : IAddEntity, new()
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                entity.IsLocalOnly = true;
                LocallyAddEntity(entity, addId: true);
                return new AddEntityResult(entity.Id, AddEntityResultEnum.AddedLocally);
            }

            HttpResult<int> result = await (asFormData
                ? restClient.PostAsFormDataAsync<int>(url, dto, parameters)
                : restClient.PostAsync<int>(url, dto, parameters));

            if (!result.IsSuccessful)
            {
                entity.IsLocalOnly = true;
                LocallyAddEntity(entity, addId: true);
                return new AddEntityResult(entity.Id, AddEntityResultEnum.AddedLocally);
            }

            entity.IsLocalOnly = false;
            entity.Id = result.Content;
            LocallyAddEntity(entity, addId: false);
            return new AddEntityResult(entity.Id, AddEntityResultEnum.AddedOnServer);
        }

        public async Task<UpdateEntityResultEnum> Update<TEntity, TDto>(int id, Action<TEntity> update, TDto dto, string url, object parameters = null, bool asFormData = false)
            where TEntity : IUpdateEntity, new()
        {
            TEntity entity;
            using (IDbContext context = contextBuilder.CreateContext())
            {
                entity = context.TLTable<TEntity>()
                    .FirstOrDefault(f => f.Id == id);
            }

            if (entity == null)
            {
                return UpdateEntityResultEnum.NotInDatabase;
            }

            update(entity);

            if (entity is IAddEntity addEntity && addEntity.IsLocalOnly)
            {
                UpdateEntityLocally(entity);
                return UpdateEntityResultEnum.NotUploadedToServer;
            }

            HttpResult result = await (asFormData
                ? restClient.PostAsFormDataAsync(url, dto, parameters)
                : restClient.PutAsync(url, dto, parameters));

            entity.HasBeenUpdatedLocally = !result.IsSuccessful;
            UpdateEntityLocally(entity);

            return result.IsSuccessful
                ? UpdateEntityResultEnum.UpdatedOnServer
                : UpdateEntityResultEnum.UpdatedLocally;
        }

        public async Task<DeleteEntityResultEnum> Delete<TEntity>(int id, string url)
            where TEntity : IDeleteEntity, new()
        {
            HttpResult result = await restClient.DeleteAsync(url, new { id });

            if (!result.IsSuccessful)
            {
                using (IDbContext context = contextBuilder.CreateContext())
                {
                    TLTableQuery<TEntity> entitySet = context.TLTable<TEntity>();
                    TEntity entity = entitySet.FirstOrDefault(f => f.Id == id);

                    // If entity is not in the database then it was already deleted
                    if (entity == null)
                    {
                        return DeleteEntityResultEnum.EntityNotFound;
                    }

                    entity.HasBeenDeletedLocally = true;
                    entitySet.Update(entity);
                }
                return DeleteEntityResultEnum.DeletedLocally;
            }

            using (IDbContext context = contextBuilder.CreateContext())
            {
                TLTableQuery<TEntity> entitySet = context.TLTable<TEntity>();
                entitySet.Remove(id);
            }
            return DeleteEntityResultEnum.DeletedOnServer;
        }

        public void AddRelatedTables<TEntity>(List<TEntity> entities)
            where TEntity : IEntity, new()
        {
            using (IDbContext context = contextBuilder.CreateContext())
            {
                TLTableQuery<TEntity> table = context.TLTable<TEntity>();

                int id = table
                    .Select(f => f.Id)
                    .OrderBy(f => f)
                    .FirstOrDefault();

                id = id > 0 ? -1 : id - 1;

                foreach (TEntity entity in entities)
                {
                    if (entity.Id != 0)
                    {
                        entity.Id = id;
                        id--;
                    }

                    table.Add(entity);
                }
            }
        }

        public void UpdateRelatedTables<TEntity, TDto>(UpdateEntityResultEnum result, List<TDto> dtos, Func<TDto, TEntity> converter, Action<TDto, TEntity> updateEntity, int foreignKey, string foreignKeyName)
            where TEntity : IOfflineEntity, new()
            where TDto : IDtoResult
        {
            PropertyInfo property = typeof(TEntity).GetProperty(foreignKeyName);

            switch (result)
            {
                case UpdateEntityResultEnum.UpdatedOnServer:
                {
                    List<TEntity> added = new List<TEntity>();
                    List<TEntity> updated = new List<TEntity>();
                    List<int> deleted = new List<int>();

                    foreach (TDto dto in dtos)
                    {
                        switch (dto.Result)
                        {
                            case DtoResultEnum.Added:
                            {
                                TEntity entity = converter(dto);
                                property.SetValue(entity, foreignKey);
                                added.Add(entity);
                                break;
                            }
                            case DtoResultEnum.Updated:
                            {
                                updated.Add(converter(dto));
                                break;
                            }
                            case DtoResultEnum.Deleted:
                            {
                                deleted.Add(dto.Id);
                                break;
                            }
                        }
                    }
                    using (IDbContext context = contextBuilder.CreateContext())
                    {
                        if (added.Count > 0)
                        {
                            context.TLTable<TEntity>().AddRange(added);
                        }

                        if (updated.Count > 0)
                        {
                            context.TLTable<TEntity>().UpdateRange(updated);
                        }

                        if (deleted.Count > 0)
                        {
                            context.TLTable<TEntity>()
                                .Where(f => deleted.Contains(f.Id))
                                .Delete();
                        }
                    }
                    break;
                }
                case UpdateEntityResultEnum.NotUploadedToServer:
                case UpdateEntityResultEnum.UpdatedLocally:
                {
                    using (IDbContext context = contextBuilder.CreateContext())
                    {
                        List<TEntity> added = new List<TEntity>();
                        List<TEntity> updated = new List<TEntity>();

                        TLTableQuery<TEntity> table = context.TLTable<TEntity>();

                        foreach (TDto dto in dtos)
                        {
                            switch (dto.Result)
                            {
                                case DtoResultEnum.Added:
                                {
                                    TEntity entity = converter(dto);
                                    property.SetValue(entity, foreignKey);
                                    entity.IsLocalOnly = true;
                                    added.Add(entity);
                                    break;
                                }
                                case DtoResultEnum.Updated:
                                {
                                    int id = dto.Id;
                                    TEntity entity = table.FirstOrDefault(f => f.Id == id);
                                    if (entity == null)
                                    {
                                        goto case DtoResultEnum.Added;
                                    }
                                    updateEntity(dto, entity);
                                    entity.HasBeenUpdatedLocally = true;
                                    updated.Add(entity);
                                    break;
                                }
                                case DtoResultEnum.Deleted:
                                {
                                    int id = dto.Id;
                                    TEntity entity = table.FirstOrDefault(f => f.Id == id);
                                    if (entity == null)
                                    {
                                        break;
                                    }
                                    entity.HasBeenDeletedLocally = true;
                                    updated.Add(entity);
                                    break;
                                }
                            }
                        }

                        if (added.Count > 0)
                        {
                            context.TLTable<TEntity>().AddRange(added);
                        }

                        if (updated.Count > 0)
                        {
                            context.TLTable<TEntity>().UpdateRange(updated);
                        }
                    }
                    break;
                }
            }
        }

        public void Update2RelatedTables<TEntity, TDto>(UpdateEntityResultEnum result, List<TDto> dtos, Func<TLTableQuery<TEntity>, List<TEntity>> getEntities, Func<TDto, TEntity> converter, Action<TDto, TEntity> updateEntity)
            where TEntity : IOfflineEntity, new()
            where TDto : IDtoBaseResult
        {
            if (result == UpdateEntityResultEnum.UpdatedLocally || result == UpdateEntityResultEnum.NotUploadedToServer)
            {
                using (IDbContext context = contextBuilder.CreateContext())
                {
                    TLTableQuery<TEntity> table = context.TLTable<TEntity>();

                    List<UpdateClass<TEntity>> entities = getEntities(table)
                        .ConvertAll(f => new UpdateClass<TEntity> { Entity = f });

                    int id = table
                        .Select(f => f.Id)
                        .OrderBy(f => f)
                        .FirstOrDefault();

                    id = id > 0 ? -1 : id - 1;

                    foreach (TDto dto in dtos)
                    {
                        UpdateClass<TEntity> tuple = entities.Find(f => f.Entity.Id == dto.Id);

                        if (tuple == null)
                        {
                            TEntity entity = converter(dto);
                            entity.IsLocalOnly = true;

                            if (entity.Id == 0)
                            {
                                entity.Id = id;
                                id--;
                            }

                            table.Add(entity);
                        }
                        else
                        {
                            tuple.Found = true;
                            tuple.Entity.HasBeenUpdatedLocally = true;
                            updateEntity(dto, tuple.Entity);
                            table.Update(tuple.Entity);
                        }
                    }

                    List<int> notFoundEntityIds = entities
                        .Where(f => !f.Found)
                        .Select(f => f.Entity.Id)
                        .ToList();

                    if (notFoundEntityIds.Count > 0)
                    {
                        table.Delete(f => notFoundEntityIds.Contains(f.Id));
                    }
                }
            }
        }

        public void DeleteRelatedTables<TEntity>(DeleteEntityResultEnum result, List<TEntity> entities)
            where TEntity : IDeleteEntity, new()
        {
            List<int> entityIds = entities.ConvertAll(f => f.Id);
            using (IDbContext context = contextBuilder.CreateContext())
            {
                switch (result)
                {
                    case DeleteEntityResultEnum.DeletedOnServer:
                        context.TLTable<TEntity>()
                            .Where(f => entityIds.Contains(f.Id))
                            .Delete();
                        break;
                    case DeleteEntityResultEnum.DeletedLocally:
                        TLTableQuery<TEntity> table = context.TLTable<TEntity>();

                        List<TEntity> entitiesFromDb = table
                            .Where(f => entityIds.Contains(f.Id))
                            .ToList();
                        foreach (TEntity entity in entitiesFromDb)
                        {
                            entity.HasBeenDeletedLocally = true;
                        }
                        table.UpdateRange(entitiesFromDb);
                        break;
                }
            }
        }

        private void LocallyAddEntity<TEntity>(TEntity entity, bool addId)
            where TEntity : IAddEntity, new()
        {
            using (IDbContext context = contextBuilder.CreateContext())
            {
                TLTableQuery<TEntity> entitySet = context.TLTable<TEntity>();
                if (addId)
                {
                    int id = entitySet
                        .Select(f => f.Id)
                        .OrderBy(f => f)
                        .FirstOrDefault();

                    entity.Id = id > 0 ? -1 : id - 1;
                }
                entitySet.Add(entity);
            }
        }

        private void UpdateEntityLocally<TEntity>(TEntity entity)
            where TEntity : IUpdateEntity, new()
        {
            using (IDbContext context = contextBuilder.CreateContext())
            {
                context.TLTable<TEntity>().Update(entity);
            }
        }
    }
}
