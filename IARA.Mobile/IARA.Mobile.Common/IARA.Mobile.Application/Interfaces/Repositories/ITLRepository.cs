using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Application.Interfaces.Repositories
{
    public interface ITLRepository
    {
        Task<AddEntityResult> Add<TEntity, TDto>(TEntity entity, TDto dto, string url, object parameters = null, bool asFormData = false)
            where TEntity : IAddEntity, new();

        Task<UpdateEntityResultEnum> Update<TEntity, TDto>(int id, Action<TEntity> update, TDto dto, string url, object parameters = null, bool asFormData = false)
            where TEntity : IUpdateEntity, new();

        Task<DeleteEntityResultEnum> Delete<TEntity>(int id, string url)
            where TEntity : IDeleteEntity, new();

        void AddRelatedTables<TEntity>(List<TEntity> entities)
            where TEntity : IEntity, new();

        void UpdateRelatedTables<TEntity, TDto>(UpdateEntityResultEnum result, List<TDto> dtos, Func<TDto, TEntity> converter, Action<TDto, TEntity> updateEntity, int foreignKey, string foreignKeyName)
            where TEntity : IOfflineEntity, new()
            where TDto : IDtoResult;

        void Update2RelatedTables<TEntity, TDto>(UpdateEntityResultEnum result, List<TDto> dtos, Func<TLTableQuery<TEntity>, List<TEntity>> getEntities, Func<TDto, TEntity> converter, Action<TDto, TEntity> updateEntity)
            where TEntity : IOfflineEntity, new()
            where TDto : IDtoBaseResult;

        void DeleteRelatedTables<TEntity>(DeleteEntityResultEnum result, List<TEntity> entities)
            where TEntity : IDeleteEntity, new();
    }
}
