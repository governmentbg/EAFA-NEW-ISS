using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IARA.RegixAbstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Tests
{
    internal class StateMachineTests : BaseTests
    {
        private static Dictionary<int, int> APPLICATIONS = new Dictionary<int, int>
        {
            //{ 9329701,  55447 }
        };

        public StateMachineTests(IServiceProvider provider) : base(provider)
        {
        }

        public void Test()
        {
            Random random = new Random();
        BEGIN:
            for (int i = 0; i < 1; i++)
            {
                var applications = APPLICATIONS.ToList();
                int index = random.Next(0, APPLICATIONS.Count);
                var appl = applications[index];
                TestRegixConclusionsService(provider, appl.Key, appl.Value);
                TestRegixConclusionsService(provider, appl.Key, appl.Value);
                Console.WriteLine($"Queued: {i}");
            }

            Thread.Sleep(10000);
            goto BEGIN;
        }


        private static Task<bool> TestRegixConclusionsService(IServiceProvider provider, int applicationId, int applicationHistoryId)
        {
            IRegixConclusionsService regixConclusionsService = provider.GetService<IRegixConclusionsService>();

            return regixConclusionsService.FinalDecision(applicationId, applicationHistoryId);
        }


    }
}
