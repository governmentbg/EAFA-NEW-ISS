using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface INomenclaturesService
    {
        IQueryable<T> GetAll<T>(NomenclaturesFilters filters) where T : class;

        int AddRecord<T>(Dictionary<string, string> record) where T : class;

        void UpdateRecord<T>(Dictionary<string, string> record) where T : class;

        void DeleteRecord<T>(int recordId) where T : class;

        void RestoreRecord<T>(int recordId) where T : class;

        SimpleAuditDTO GetAuditInfoForTable(int tableId, int id);

        List<NomenclatureDTO> GetGroups();

        IQueryable<NomenclatureTableDTO> GetTables();

        List<PermissionTypeEnum> GetTablePermissions(int tableId);

        List<ColumnDTO> GetColumns(int tableId);

        Dictionary<string, List<NomenclatureDTO>> GetChildNomenclatures(int tableId);

        Type GetEntityType(int tableId);

        T GetRecordById<T>(int id) where T : class;
    }
}
