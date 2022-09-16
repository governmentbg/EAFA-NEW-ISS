using System;
using IARA.Interfaces.CrossChecks;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Tests.Tests
{
    internal class CrossChecksTests : BaseTests
    {
        public CrossChecksTests(IServiceProvider provider)
            : base(provider)
        { }

        public void Test()
        {
            ICrossChecksExecutionService crossService = provider.GetService<ICrossChecksExecutionService>();
            crossService.ExecuteCrossChecks("Daily");
            crossService.ExecuteCrossChecks("Montly");
        }
    }
}
