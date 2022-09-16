using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class NomenclatureTests
    {
        private IARADbContext Db;
        private INomenclaturesService service;

        public NomenclatureTests(IARADbContext Db, INomenclaturesService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.Db = Db;
            this.service = service;

            this.Db.NaddressTypes.AddRange(AddressData.AddressTypes);
            this.Db.Ndistricts.AddRange(AddressData.Districts);
            this.Db.Nmunicipalities.AddRange(AddressData.Municipalities);
            this.Db.NpopulatedAreas.AddRange(AddressData.PopulatedAreas);
            this.Db.NterritoryUnits.AddRange(TerritoryData.TerritoryUnits);
            this.Db.NnomenclatureTables.AddRange(NNomenclatureTables.NomenclatureTables);
            this.Db.SaveChanges();
        }

        [Fact(DisplayName = "Тества извличане на всички номенклатурни таблици")]
        public void TestGetTables()
        {
            service.GetTables();
        }

        [Fact(DisplayName = "Тества извличане на всички номенклатури от таблица")]
        public void TestGetAllEmptyFilter()
        {
            NomenclaturesFilters filter = new NomenclaturesFilters();
            int tableId = Db.NnomenclatureTables.Where(x => x.Name == "NMunicipalities").First().Id;
            filter.TableId = tableId;
            var result = service.GetAll<Nmunicipality>(filter);
            var data = AddressData.Municipalities.Where(x => x.ValidFrom < DateTime.Now && x.ValidTo > DateTime.Now).ToList();

            Assert.NotNull(result);
            // Assert.True(result.All(x => CommonUtils.IsEntryActive(x)));
            Assert.Equal(data.Count, result.Count());
        }

        [Fact(DisplayName = "Тества извличане на номенклатури с филтър")]
        public void TestGetAllFreeTextFilter()
        {
            NomenclaturesFilters filter = new NomenclaturesFilters();
            filter.FreeTextSearch = "Столи";
            int tableId = Db.NnomenclatureTables.Where(x => x.Name == "NMunicipalities").First().Id;
            filter.TableId = tableId;
            var result = service.GetAll<Nmunicipality>(filter);
            var data = AddressData.Municipalities.Where(x => x.ValidFrom < DateTime.Now && x.ValidTo > DateTime.Now && (x.Name.ToLower().Contains(filter.FreeTextSearch.ToLower()) || x.Code.ToLower().Contains(filter.FreeTextSearch.ToLower()))).ToList();

            Assert.NotNull(result);
            //Assert.True(result.All(x => CommonUtils.IsEntryActive(x.i)));
            Assert.Equal(data.Count, result.Count());
        }

        [Fact(DisplayName = "Тества добавяне на номенклатура")]
        public void TestAdd()
        {
            int tableId = Db.NnomenclatureTables.Where(x => x.Name == "NMunicipalities").First().Id;
            Dictionary<string, string> newEntryData = new Dictionary<string, string>
            {
                 { "Id", "534" },
                 { "Code", "BGS01" },
                 { "Name", "Айтос" },
                 { "DistrictId", "3" }
            };

            service.AddRecord<Nmunicipality>(newEntryData);
            NomenclaturesFilters filter = new NomenclaturesFilters();
            filter.FreeTextSearch = "Айтос";
            filter.TableId = tableId;
            var results = service.GetAll<Nmunicipality>(filter);

            var result = results.FirstOrDefault();
            Assert.NotNull(result);

        }

        [Fact(DisplayName = "Тества редакция на номенклатура")]
        public void TestEdit()
        {
            int tableId = Db.NnomenclatureTables.Where(x => x.Name == "NMunicipalities").First().Id;
            Dictionary<string, string> newDataPair = new Dictionary<string, string> { { "Id", "737" }, { "Name", "Силистра1" } };

            int newId = service.UpdateRecord<Nmunicipality>(newDataPair);

            var newValue = service.GetRecordById<Nmunicipality>(newId);

            Assert.Equal(newValue.Name, newDataPair.First().Value);

            int populatedAreaTableId = Db.NnomenclatureTables.Where(x => x.Name == "NPopulatedAreas").First().Id;
            var populatedArea = service.GetRecordById<NpopulatedArea>(10802);

            Assert.Equal(populatedArea.MunicipalityId, newId);
        }
    }
}
