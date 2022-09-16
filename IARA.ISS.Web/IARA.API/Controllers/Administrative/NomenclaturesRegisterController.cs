using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class NomenclaturesRegisterController : BaseController
    {
        private readonly INomenclaturesService service;

        public NomenclaturesRegisterController(INomenclaturesService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<NomenclaturesFilters> request)
        {
            if (service.GetTablePermissions(request.Filters.TableId).Contains(PermissionTypeEnum.READ))
            {
                Type entityType = service.GetEntityType(request.Filters.TableId);

                object result = CallServiceMethod(nameof(INomenclaturesService.GetAll), entityType, request.Filters);
                return PageResult(result, entityType, request);
            }

            return Forbid();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult Get([FromQuery] int tableId, [FromQuery] int id)
        {
            Type entityType = service.GetEntityType(tableId);
            return Ok(nameof(INomenclaturesService.GetRecordById), entityType, id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.NomenclaturesAddRecords)]
        public IActionResult Add([FromQuery] int tableId, [FromBody] Dictionary<string, string> record)
        {
            if (service.GetTablePermissions(tableId).Contains(PermissionTypeEnum.ADD))
            {
                Type entityType = this.service.GetEntityType(tableId);
                return Ok(nameof(INomenclaturesService.AddRecord), entityType, record);
            }

            return Forbid();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.NomenclaturesEditRecords)]
        public IActionResult Edit([FromQuery] int tableId, [FromBody] Dictionary<string, string> recordValues)
        {
            if (service.GetTablePermissions(tableId).Contains(PermissionTypeEnum.EDIT))
            {
                Type entityType = service.GetEntityType(tableId);
                return Ok(nameof(INomenclaturesService.UpdateRecord), entityType, recordValues);
            }

            return Forbid();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.NomenclaturesDeleteRecords)]
        public IActionResult Delete(int tableId, int id)
        {
            if (service.GetTablePermissions(tableId).Contains(PermissionTypeEnum.DELETE))
            {
                Type entityType = service.GetEntityType(tableId);
                return Ok(nameof(INomenclaturesService.DeleteRecord), entityType, id);
            }

            return Forbid();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.NomenclaturesRestoreRecords)]
        public IActionResult Restore(int tableId, int id)
        {
            if (service.GetTablePermissions(tableId).Contains(PermissionTypeEnum.RESTORE))
            {
                Type entityType = service.GetEntityType(tableId);
                return Ok(nameof(INomenclaturesService.RestoreRecord), entityType, id);
            }

            return Forbid();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetGroups()
        {
            List<NomenclatureDTO> groups = service.GetGroups();
            return Ok(groups);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetTables()
        {
            List<NomenclatureTableDTO> tables = service.GetTables().ToList();
            return Ok(tables);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetTablePermissions(int tableId)
        {
            IEnumerable<PermissionTypeEnum> permissions = service.GetTablePermissions(tableId);
            return Ok(permissions);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetColumns(int tableId)
        {
            List<ColumnDTO> columns = service.GetColumns(tableId);
            return Ok(columns);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetChildNomenclatures(int tableId)
        {
            Dictionary<string, List<NomenclatureDTO>> result = service.GetChildNomenclatures(tableId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NomenclaturesRead)]
        public IActionResult GetAuditInfoForTable(int tableId, int id)
        {
            SimpleAuditDTO audit = service.GetAuditInfoForTable(tableId, id);
            return Ok(audit);
        }

        private IActionResult PageResult(object result, Type entityType, GridRequestModel<NomenclaturesFilters> request)
        {
            return typeof(NomenclaturesRegisterController)
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(x => x.IsGenericMethod && x.Name == nameof(PageResult))
                        .First()
                        .MakeGenericMethod(entityType)
                        .Invoke(this, new object[] { result, request, true }) as IActionResult;
        }

        private IActionResult Ok(string methodName, Type type, params object[] parameters)
        {
            object value = CallServiceMethod(methodName, type, parameters);
            return value != null ? Ok(value) : Ok();
        }

        private object CallServiceMethod(string methodName, Type type, params object[] parameters)
        {
            try
            {
                return typeof(INomenclaturesService)
                    .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)!
                    .MakeGenericMethod(type)
                    .Invoke(this.service, parameters);
            }
            catch (Exception ex)
            {
                Exception innerEx = ex;

                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                throw innerEx;
            }
        }
    }
}
