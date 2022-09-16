using System;
using System.Security.Claims;
using System.Threading;
using IARA.DomainModels.DTOModels;
using IARA.Fakes.MockupData;
using IARA.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace IARA.Web.IntegrationTests
{
    public class PoundnetRegisterIntegrationTests
    {
        private PoundNetRegisterController controller;

        public PoundnetRegisterIntegrationTests(PoundNetRegisterController controller)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.controller = controller;
        }

        [Fact(DisplayName = "Тест за добавяне на нов запис в регистър Даляни")]
        public void TestAddPoundnetRegister()
        {
            var result = controller.Add(null);

            Assert.True(result is UnprocessableEntityResult);

            var poundRegister = new PoundnetRegisterDTO { };

            result = controller.Add(poundRegister);

            Assert.True(result is UnprocessableEntityResult);

            poundRegister.Name = "gaegae";
            poundRegister.SeasonTypeId = PoundnetRegisterData.PoundnetSeasonTypes[0].Id;
            poundRegister.CategoryTypeId = PoundnetRegisterData.PoundnetCategoryTypes[0].Id;
            poundRegister.RegistrationDate = DateTime.Now;
            poundRegister.PoundNetNum = "gaegae";

            result = controller.Add(poundRegister);

            Assert.True(result is OkObjectResult);
        }
    }
}
