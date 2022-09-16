using System;

namespace IARA.Tests
{
    internal class BaseTests
    {
        protected readonly IServiceProvider provider;

        public BaseTests(IServiceProvider provider)
        {
            this.provider = provider;
        }

    }
}
