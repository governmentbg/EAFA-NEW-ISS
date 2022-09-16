using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IARA.Infrastructure.Services
{
    public class NomenclaturesService : BaseService, INomenclaturesService
    {
        private static readonly string[] IGNORED_PROPERTIES = new string[] {
            nameof(IIdentityRecord.Id),
            nameof(IAuditEntity.CreatedBy),
            nameof(IAuditEntity.CreatedOn),
            nameof(IAuditEntity.UpdatedBy),
            nameof(IAuditEntity.UpdatedOn),
        };

        private List<Type> dbsetProperties;

        public NomenclaturesService(IARADbContext db)
            : base(db)
        { }

        protected List<Type> DbSetPropertyTypes
        {
            get
            {
                if (dbsetProperties == null)
                {
                    dbsetProperties = typeof(IARADbContext)
                     .GetProperties()
                     .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                     .Select(x => x.PropertyType.GetGenericArguments()[0])
                     .ToList();
                }

                return dbsetProperties;
            }
        }

        public IQueryable<T> GetAll<T>(NomenclaturesFilters filters)
            where T : class
        {
            DbSet<T> dbSet = Db.Set<T>();
            IQueryable<T> query = null;

            if (dbSet.Any())
            {
                query = dbSet;

                if (HasInterface<T, IValidity>())
                {
                    query = CallFilterMethod(query, nameof(NomenclaturesService.FilterValidityRecords), filters);
                }

                if (HasInterface<T, ISoftDeletable>())
                {
                    query = CallFilterMethod(query, nameof(NomenclaturesService.FilterSoftDeletableRecords), filters);
                }

                if (filters.HasAnyFilters(true))
                {
                    if (HasInterface<T, INomenclature>() && !string.IsNullOrEmpty(filters.Name))
                    {
                        filters.Name = filters.Name.ToLower();
                        query = CallFilterMethod(query, nameof(NomenclaturesService.FilterNomenclatureByName), filters);
                    }

                    if (HasInterface<T, ICodeEntity>() && !string.IsNullOrEmpty(filters.Code))
                    {
                        filters.Code = filters.Code.ToLower();
                        query = CallFilterMethod(query, nameof(NomenclaturesService.FilterDbSetByCode), filters);
                    }
                }
                else if (filters.HasFreeTextSearch())
                {
                    filters.FreeTextSearch = filters.FreeTextSearch.ToLower();
                    query = CallFilterMethod(query, nameof(NomenclaturesService.FilterByFreeText), filters);
                }

                return Select(query, GetSimplePropertyNames<T>());
            }

            return Enumerable.Empty<T>().AsQueryable();
        }

        public int AddRecord<T>(Dictionary<string, string> record)
            where T : class
        {
            Dictionary<string, string> newRecord = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> property in record)
            {
                newRecord.Add(ToPascalCase(property.Key), property.Value);
            }

            record = newRecord;

            T instance = Activator.CreateInstance<T>();
            FillObjectValues(record, instance);
            OnNomenclatureUpdated<T>();
            instance = CallGenericServiceMethod<T>(nameof(NomenclaturesService.AddEntity), instance, false) as T;
            return (instance as IIdentityRecord).Id;
        }

        public void UpdateRecord<T>(Dictionary<string, string> record)
            where T : class
        {
            OnNomenclatureUpdated<T>();
            CallGenericServiceMethod<T>(nameof(NomenclaturesService.UpdateEntry), record);
        }

        public void DeleteRecord<T>(int recordId)
            where T : class
        {
            DbSet<T> dbSet = Db.Set<T>();

            IQueryable<T> query = CallFilterMethod(dbSet, nameof(NomenclaturesService.FilterById), recordId, true);
            T recordToDelete = query.First();

            if (HasInterface<T, ISoftDeletable>())
            {
                (recordToDelete as ISoftDeletable).IsActive = false;
            }
            else if (HasInterface<T, IValidity>())
            {
                IValidity record = (recordToDelete as IValidity);
                record.ValidTo = DateTime.Now;
            }

            OnNomenclatureUpdated<T>();
            Db.SaveChanges();
        }

        public void RestoreRecord<T>(int recordId)
            where T : class
        {
            DbSet<T> dbSet = Db.Set<T>();
            IQueryable<T> query = CallFilterMethod<T>(dbSet, nameof(NomenclaturesService.FilterById), recordId, false);

            if (HasInterface<T, ISoftDeletable>())
            {
                T recordToRestore = query.First();
                (recordToRestore as ISoftDeletable).IsActive = true;
            }
            else if (HasInterface<T, IValidity>())
            {
                query = CallGenericServiceMethod<T>(nameof(NomenclaturesService.OrderByValidity), query, false) as IQueryable<T>;
                T lastRecord = query.First();

                // TODO fix collisions ??

                CreateActiveDuplicate(lastRecord);
            }

            OnNomenclatureUpdated<T>();
            Db.SaveChanges();
        }

        public SimpleAuditDTO GetAuditInfoForTable(int tableId, int id)
        {
            string tableName = GetTableName(tableId);
            Type entityType = GetEntityType(tableName);

            try
            {
                IAuditEntity entity = GetSingle(entityType, id) as IAuditEntity;
                return new SimpleAuditDTO
                {
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedOn = entity.UpdatedOn
                };
            }
            catch (Exception ex)
            {
                throw new RecordNotFoundException("Record not found", ex);
            }
        }

        public List<NomenclatureDTO> GetGroups()
        {
            List<NomenclatureDTO> groups = (from gr in Db.NnomenclatureGroups
                                            where gr.IsActive
                                            orderby gr.OrderNo
                                            select new NomenclatureDTO
                                            {
                                                Value = gr.Id,
                                                DisplayName = gr.Name,
                                                IsActive = gr.IsActive
                                            }).ToList();

            return groups;
        }

        public IQueryable<NomenclatureTableDTO> GetTables()
        {
            IQueryable<NomenclatureTableDTO> query = from table in Db.NnomenclatureTables
                                                     where table.IsActive
                                                     orderby table.Name
                                                     select new NomenclatureTableDTO
                                                     {
                                                         Value = table.Id,
                                                         TableName = table.Name,
                                                         DisplayName = table.Description,
                                                         GroupId = table.GroupId,
                                                         CanInsertRows = table.CanInsertRows.HasValue && table.CanInsertRows.Value,
                                                         CanEditRows = table.CanEditRows.HasValue && table.CanEditRows.Value,
                                                         CanDeleteRows = table.CanDeleteRows.HasValue && table.CanDeleteRows.Value,
                                                         IsActive = table.IsActive
                                                     };

            return query;
        }

        public List<PermissionTypeEnum> GetTablePermissions(int tableId)
        {
            List<PermissionTypeEnum> permissions = new List<PermissionTypeEnum> { PermissionTypeEnum.READ };
            NomenclatureTableDTO table = GetTables().Where(x => x.Value == tableId).First();

            if (table.CanInsertRows)
            {
                permissions.Add(PermissionTypeEnum.ADD);
            }

            if (table.CanEditRows)
            {
                permissions.Add(PermissionTypeEnum.EDIT);
            }

            if (table.CanDeleteRows)
            {
                permissions.Add(PermissionTypeEnum.DELETE);
                permissions.Add(PermissionTypeEnum.RESTORE);
            }

            return permissions;
        }

        public List<ColumnDTO> GetColumns(int tableId)
        {
            Type entityType = GetEntityType(tableId);
            List<ColumnDTO> columns = new List<ColumnDTO>();

            IEnumerable<PropertyInfo> simpleProperties = entityType.GetProperties()
                           .Where(x => x.PropertyType.IsValueType
                                    || x.PropertyType == typeof(string)
                                    || (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)));


            List<string> foreignKeyProperties = entityType.GetProperties()
                     .Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null)
                     .Select(x => x.GetCustomAttribute<ForeignKeyAttribute>().Name)
                     .ToList();

            foreach (PropertyInfo property in simpleProperties.Where(x => !IGNORED_PROPERTIES.Contains(x.Name)))
            {
                ColumnDTO column = new ColumnDTO
                {
                    DataType = GetDisplayType(property),
                    PropertyName = ToCamelCase(property.Name),
                    DisplayName = GetColumnProperty(entityType, property.Name)?.GetComment() ?? property.Name,
                    IsRequired = IsPropertyRequired(property),
                    MaxLength = MaxLength(property),
                    IsForeignKey = foreignKeyProperties.Contains(property.Name)
                };

                if (column.IsForeignKey)
                {
                    column.DataType = "nomenclature";
                }

                columns.Add(column);
            }

            return columns;
        }

        public Dictionary<string, List<NomenclatureDTO>> GetChildNomenclatures(int tableId)
        {
            string tableName = GetTableName(tableId);
            Dictionary<string, List<NomenclatureDTO>> nomenclatures = new Dictionary<string, List<NomenclatureDTO>>();
            Type type = GetEntityType(tableName);

            IEnumerable<PropertyInfo> navigationProperties = type.GetProperties().Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null);

            foreach (PropertyInfo prop in navigationProperties)
            {
                string foreignKey = prop.GetCustomAttribute<ForeignKeyAttribute>().Name;

                List<NomenclatureDTO> nomenclature = CallServiceMethod(nameof(NomenclaturesService.GetNomenclatureByType), prop.PropertyType) as List<NomenclatureDTO>;
                if (nomenclature != null)
                {
                    nomenclatures.Add(ToCamelCase(foreignKey), nomenclature);
                }
            }

            return nomenclatures;
        }

        public Type GetEntityType(int tableId)
        {
            string tableName = GetTableName(tableId);
            Type t = GetEntityType(tableName);

            return t ?? throw new ArgumentException($"Ненамерен модел на entity в Db context-a за таблица {tableName}", nameof(tableId));
        }

        public T GetRecordById<T>(int id)
            where T : class
        {
            return GetSingle(typeof(T), id) as T;
        }

        private IQueryable<T> CallFilterMethod<T>(IQueryable<T> dbSet, string methodName, NomenclaturesFilters filter)
            where T : class
        {
            return CallGenericServiceMethod<T>(methodName, dbSet, filter) as IQueryable<T>;
        }

        private IQueryable<T> CallFilterMethod<T>(IQueryable<T> dbSet, string methodName, params object[] parameters)
            where T : class
        {
            object[] tempParams = new object[parameters.Length + 1];

            tempParams[0] = dbSet;

            for (int i = 0; i < parameters.Length; i++)
            {
                tempParams[i + 1] = parameters[i];
            }

            return CallGenericServiceMethod<T>(methodName, tempParams) as IQueryable<T>;
        }

        private static IQueryable<T> FilterByFreeText<T>(IQueryable<T> dbset, NomenclaturesFilters filters)
            where T : class
        {
            bool hasName = HasInterface<T, INomenclature>();
            bool hasCode = HasInterface<T, ICodeEntity>();
            bool hasDescr = HasInterface<T, IDescribedEntity>();
            bool hasValidity = HasInterface<T, IValidity>();

            dbset = dbset.Where(x => (hasName && (x as INomenclature).Name.ToLower().Contains(filters.FreeTextSearch))
                                 || (hasCode && (x as ICodeEntity).Code.ToLower().Contains(filters.FreeTextSearch))
                                 || (hasDescr && (x as IDescribedEntity).Description.ToLower().Contains(filters.FreeTextSearch))
                                 || (hasValidity && ((x as IValidity).ValidFrom.ToString().Contains(filters.FreeTextSearch) || (x as IValidity).ValidTo.ToString().Contains(filters.FreeTextSearch))));

            return dbset;
        }

        private IQueryable<T> FilterById<T>(IQueryable<T> dbSet, int recordId, bool filterByValidity = true)
            where T : class, IIdentityRecord
        {
            IQueryable<T> query = dbSet.Where(x => x.Id == recordId);

            DateTime now = DateTime.Now;
            if (filterByValidity)
            {
                if (HasInterface<T, IValidity>())
                {
                    query = CallFilterMethod(query, nameof(NomenclaturesService.FilterValidityRecords), new NomenclaturesFilters
                    {
                        ValidityDateFrom = now,
                        ValidityDateTo = now
                    });
                }
                else if (HasInterface<T, ISoftDeletable>())
                {
                    query = CallFilterMethod(query, nameof(NomenclaturesService.FilterSoftDeletableRecords), new NomenclaturesFilters
                    {
                        ShowInactiveRecords = false
                    });
                }
            }

            return query;
        }

        private T AddEntity<T>(T entity, bool deferSave = false)
            where T : class, IIdentityRecord
        {
            entity.Id = default;

            if (HasInterface<T, ISoftDeletable>())
            {
                (entity as ISoftDeletable).IsActive = true;
            }
            else if (HasInterface<T, IValidity>())
            {
                (entity as IValidity).ValidFrom = DateTime.Now;
                (entity as IValidity).ValidTo = DefaultConstants.MAX_VALID_DATE;
            }

            Db.Set<T>().Add(entity);

            if (!deferSave)
            {
                Db.SaveChanges();
            }

            return entity;
        }

        private void UpdateEntry<T>(Dictionary<string, string> record)
            where T : class, IIdentityRecord
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeOption.Required);

            Dictionary<string, string> newRecord = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> property in record)
            {
                newRecord.Add(ToPascalCase(property.Key), property.Value);
            }

            record = newRecord;

            // TODO fix collisions

            int entryId = int.Parse(record[nameof(IIdentityRecord.Id)]);
            T entry = this.GetRecordById<T>(entryId);
            T newEntry = CopyRecord(entry);
            DeleteRecord<T>(entryId);
            FillObjectValues(record, newEntry);
            newEntry = AddEntity(newEntry, true);

            Db.SaveChanges();

            scope.Complete();
        }

        private void OnNomenclatureUpdated<T>()
        {
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>().Name;

            NnomenclatureTable nomenclature = Db.NnomenclatureTables.Where(x => x.Name == tableName).First();
            nomenclature.DataLastEditOn = DateTime.Now;
        }

        private static IQueryable<T> FilterDbSetByCode<T>(IQueryable<T> dbset, NomenclaturesFilters filters)
            where T : class, ICodeEntity
        {
            dbset = dbset.Where(x => x.Code.ToLower().Contains(filters.Code));
            return dbset;
        }

        private static IQueryable<T> FilterNomenclatureByName<T>(IQueryable<T> dbset, NomenclaturesFilters filters)
            where T : class, INomenclature
        {
            dbset = dbset.Where(x => x.Name.ToLower().Contains(filters.Name));
            return dbset;
        }

        private static IQueryable<T> FilterSoftDeletableRecords<T>(IQueryable<T> dbset, NomenclaturesFilters filters)
            where T : class, ISoftDeletable
        {
            return dbset.Where(x => x.IsActive != filters.ShowInactiveRecords);
        }

        private static IQueryable<T> FilterValidityRecords<T>(IQueryable<T> dbSet, NomenclaturesFilters filters)
            where T : class, IValidity
        {
            DateTime now = DateTime.Now;

            IQueryable<T> query = dbSet;

            if (filters.ShowInactiveRecords)
            {
                query = query.Where(x => x.ValidTo <= now || x.ValidFrom >= now);
            }
            else
            {
                query = query.Where(x => (!filters.ValidityDateFrom.HasValue
                                         && !filters.ValidityDateTo.HasValue
                                         && x.ValidTo >= now
                                         && x.ValidFrom <= now)
                                     || (filters.ValidityDateFrom.HasValue
                                         && filters.ValidityDateTo.HasValue
                                         && x.ValidFrom <= filters.ValidityDateFrom.Value
                                         && x.ValidTo >= filters.ValidityDateTo)
                                     || (filters.ValidityDateFrom.HasValue
                                         && x.ValidFrom <= filters.ValidityDateFrom.Value)
                                     || (filters.ValidityDateTo.HasValue
                                         && x.ValidTo >= filters.ValidityDateTo));
            }

            return query;
        }

        private static string ToCamelCase(string name)
        {
            return name[0].ToString().ToLower() + name.Substring(1, name.Length - 1);
        }

        private static string ToPascalCase(string name)
        {
            return name[0].ToString().ToUpper() + name.Substring(1, name.Length - 1);
        }

        private static void FillObjectValues(Dictionary<string, string> record, object newEntity)
        {
            Dictionary<string, PropertyInfo> properties = newEntity.GetType()
                .GetProperties()
                .Select(x => x)
                .ToDictionary(x => x.Name.ToLower(), x => x);

            foreach (KeyValuePair<string, string> property in record)
            {
                PropertyInfo propertyInfo = properties.GetValueOrDefault(property.Key.ToLower());

                if (propertyInfo != null && !string.IsNullOrWhiteSpace(property.Value))
                {
                    Type targetType = propertyInfo.PropertyType;
                    if (targetType.Name == "Nullable`1")
                    {
                        targetType = targetType.GenericTypeArguments.First();
                    }

                    object value = Convert.ChangeType(property.Value, targetType);
                    propertyInfo.SetValue(newEntity, value);
                }
            }
        }

        private static T CopyRecord<T>(T record)
        {
            T newRecord = Activator.CreateInstance<T>();

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                property.SetValue(newRecord, property.GetValue(record));
            }

            return newRecord;
        }

        private static object[] FlatParamters(object[] parameters)
        {
            object[] tempParameters = null;

            if (parameters != null && parameters.Length > 0)
            {
                if (parameters[0].GetType() == typeof(object[]))
                {
                    tempParameters = parameters[0] as object[];
                }
                else
                {
                    tempParameters = parameters;
                }
            }

            return tempParameters;
        }

        private static List<string> GetSimplePropertyNames<T>()
        {
            return typeof(T).GetProperties()
                        .Where(x => x.PropertyType.IsValueType
                                 || x.PropertyType == typeof(string)
                                 || (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        .Select(x => x.Name)
                        .ToList();
        }

        private static string GetDisplayType(PropertyInfo property)
        {
            Type t = property.PropertyType;

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = t.GetGenericArguments()[0];
            }

            if (t == typeof(bool))
            {
                return "boolean";
            }
            else if (t == typeof(int) || t == typeof(decimal) || t == typeof(double))
            {
                return "number";
            }
            else if (t == typeof(DateTime) || t == typeof(Date))
            {
                return "date";
            }
            else
            {
                return "string";
            }
        }

        private static int? MaxLength(PropertyInfo prop)
        {
            StringLengthAttribute attribute = prop.GetCustomAttribute<StringLengthAttribute>();
            return attribute != null ? attribute.MaximumLength : default(int?);
        }

        private static bool IsPropertyRequired(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<RequiredAttribute>() != null
                || (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>));
        }

        private static IQueryable<T> Select<T>(IQueryable<T> query, IEnumerable<string> columns)
        {
            Type t = typeof(T);

            ParameterExpression itemParam = Expression.Parameter(t, "x");

            ConstructorInfo ctr = t.GetConstructor(new Type[] { });

            List<Expression> propertiesAssignment = new List<Expression>();

            NewExpression newExp = Expression.New(ctr);

            List<MemberAssignment> members = t.GetMembers()
                                              .Where(x => columns.Contains(x.Name))
                                              .Select(x => Expression.Bind(x, Expression.Property(itemParam, x.Name)))
                                              .ToList();

            Expression<Func<T, T>> lambda = Expression.Lambda<Func<T, T>>(Expression.MemberInit(newExp, members), itemParam);
            return query.Select(lambda);
        }

        private static bool HasInterface<T>(Type interfaceType)
        {
            return typeof(T).GetInterfaces().Where(x => x == interfaceType).Any();
        }

        private static bool HasInterface<TEntity, TInterface>()
        {
            return typeof(TEntity).GetInterfaces().Where(x => x == typeof(TInterface)).Any();
        }

        private static IQueryable<T> OrderByValidity<T>(IQueryable<T> query, bool asc = true)
            where T : class, IValidity
        {
            return asc ? query.OrderBy(x => x.ValidFrom) : query.OrderByDescending(x => x.ValidFrom);
        }

        private object CallGenericServiceMethod<T>(string methodName, params object[] parameters)
        {
            object[] tempParameters = FlatParamters(parameters);
            return CallServiceMethod(methodName, typeof(T), tempParameters);
        }

        private object CallServiceMethod(string methodName, Type entityType, params object[] parameters)
        {
            object[] tempParameters = FlatParamters(parameters);

            if (tempParameters == null)
            {
                tempParameters = new object[] { };
            }

            MethodInfo method = typeof(NomenclaturesService)
              .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            return method.MakeGenericMethod(entityType).Invoke(this, tempParameters);
        }

        private void CreateActiveDuplicate<T>(T lastRecord)
          where T : class
        {
            T newRecord = CopyRecord(lastRecord);

            (newRecord as INomenclature).Id = default;
            (newRecord as IValidity).ValidFrom = DateTime.Now;
            (newRecord as IValidity).ValidTo = DefaultConstants.MAX_VALID_DATE;

            Db.Set<T>().Add(newRecord);
        }

        private IProperty GetColumnProperty(Type entityType, string propertyName)
        {
            IEnumerable<IEntityType> etypes = Db.Model.GetEntityTypes().Where(x => x.ClrType == entityType);

            IEntityType etype = etypes.FirstOrDefault();
            if (etype != null)
            {
                IEnumerable<IProperty> eproperties = etype.GetProperties().Where(x => x.Name == propertyName);
                IProperty eproperty = eproperties.FirstOrDefault();
                return eproperty;
            }

            return null;
        }

        private Type GetEntityType(string tableName)
        {
            foreach (Type dbSetType in DbSetPropertyTypes)
            {
                TableAttribute tableAttribute = dbSetType.GetCustomAttribute<TableAttribute>();

                if (tableAttribute.Name == tableName)
                {
                    return dbSetType;
                }
            }

            return null;
        }

        private string GetTableName(int tableId)
        {
            string tableName = (from table in Db.NnomenclatureTables
                                where table.Id == tableId
                                select table.Name).First();

            return tableName;
        }

        private IIdentityRecord GetSingle(Type entityType, int id)
        {
            IQueryable<IIdentityRecord> entries = GetEntries(entityType);

            IIdentityRecord record = entries.Where(x => x.Id == id).FirstOrDefault();
            return record;
        }

        private IQueryable<IIdentityRecord> GetEntries(Type entityType)
        {
            string propertyName = Db.GetType().GetProperties().Where(x => x.PropertyType.GetGenericArguments().Contains(entityType)).FirstOrDefault()?.Name;

            IQueryable<IIdentityRecord> result;
            if (propertyName != null)
            {
                result = (Db.GetType().GetProperty(propertyName).GetValue(Db) as IQueryable<IIdentityRecord>);
            }
            else
            {
                result = Enumerable.Empty<IIdentityRecord>().AsQueryable();
            }

            return result;
        }

        private List<NomenclatureDTO> GetCodeSoftDeletableNomenclature<T>()
            where T : class, INomenclature, ICodeEntity, ISoftDeletable
        {
            List<NomenclatureDTO> result = (from entry in Db.Set<T>()
                                            orderby entry.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = entry.Id,
                                                Code = entry.Code,
                                                DisplayName = entry.Name,
                                                IsActive = entry.IsActive
                                            }).ToList();

            return result;
        }

        private List<NomenclatureDTO> GetCodeValidityNomenclature<T>()
            where T : class, INomenclature, ICodeEntity, IValidity
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from entry in Db.Set<T>()
                                            orderby entry.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = entry.Id,
                                                Code = entry.Code,
                                                DisplayName = entry.Name,
                                                IsActive = entry.ValidFrom <= now && entry.ValidTo > now
                                            }).ToList();

            return result;
        }

        private List<NomenclatureDTO> GetNomenclatureByType<T>()
        {
            if (HasInterface<T>(typeof(INomenclature)))
            {
                if (HasInterface<T>(typeof(ICodeEntity)) && HasInterface<T>(typeof(IValidity)))
                {
                    return CallGenericServiceMethod<T>(nameof(NomenclaturesService.GetCodeValidityNomenclature)) as List<NomenclatureDTO>;
                }
                else if (HasInterface<T>(typeof(ICodeEntity)) && HasInterface<T>(typeof(ISoftDeletable)))
                {
                    return CallGenericServiceMethod<T>(nameof(NomenclaturesService.GetCodeSoftDeletableNomenclature)) as List<NomenclatureDTO>;
                }
                else if (HasInterface<T>(typeof(ISoftDeletable)))
                {
                    return CallGenericServiceMethod<T>(nameof(NomenclaturesService.GetSoftDeletableNomenclature)) as List<NomenclatureDTO>;
                }
                else if (HasInterface<T>(typeof(IValidity)))
                {
                    return CallGenericServiceMethod<T>(nameof(NomenclaturesService.GetValidityNomenclature)) as List<NomenclatureDTO>;
                }
            }

            return null;
        }

        private List<NomenclatureDTO> GetSoftDeletableNomenclature<T>()
            where T : class, INomenclature, ISoftDeletable
        {
            List<NomenclatureDTO> result = (from entry in Db.Set<T>()
                                            orderby entry.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = entry.Id,
                                                DisplayName = entry.Name,
                                                IsActive = entry.IsActive
                                            }).ToList();

            return result;
        }

        private List<NomenclatureDTO> GetValidityNomenclature<T>()
            where T : class, INomenclature, IValidity
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from entry in Db.Set<T>()
                                            orderby entry.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = entry.Id,
                                                DisplayName = entry.Name,
                                                IsActive = entry.ValidFrom <= now && entry.ValidTo > now
                                            }).ToList();

            return result;
        }
    }
}
