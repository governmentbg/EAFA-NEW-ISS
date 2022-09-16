using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TL.SysToSysSecCom;

namespace IARA.Flux.Tests
{
    internal static class FluxGenerateMethodTests
    {
        public static Task RunMethods(IServiceProvider serviceProvider)
        {
            ISecureHttpClient secureHttpClient = serviceProvider.GetRequiredService<ISecureHttpClient>();

            secureHttpClient.Timeout = TimeSpan.FromMinutes(10);

            return FluxEndpointTests(secureHttpClient).ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    Console.Clear();
                    Console.WriteLine("Method executed successfully!");
                    var position = Console.GetCursorPosition();
                    Console.SetCursorPosition(position.Left, position.Top + 2);
                    RunMethods(serviceProvider);
                }
            });
        }

        public static Task FluxEndpointTests(this ISecureHttpClient secureHttpClient)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("1. Fishing Activities");
            builder.AppendLine("2. Vessels");
            builder.AppendLine("3. NISS");

            Console.Write(builder.ToString());
            Console.WriteLine();
            Console.Write("Select number: ");

            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    return FluxFishingActivityTests(secureHttpClient);
                case 2:
                    return FluxVesselTests(secureHttpClient);
                case 3:
                    return FluxNISSTests(secureHttpClient);
                default:
                    return Task.CompletedTask;
            }
        }


        public static async Task FluxNISSTests(this ISecureHttpClient secureHttpClient)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("1. GenFAReport_Departure_FVMS_To_ISS");
            builder.AppendLine("2. GenFAReport_Daily_FVMS_To_ISS");
            builder.AppendLine("3. GenFAReport_ReportPriorNotifOfArrivalPortLanding_FVMS_To_ISS");
            builder.AppendLine("4. GenFAReport_LandingDeclaration_FVMS_To_ISS");

            Console.Write(builder.ToString());
            Console.WriteLine();
            Console.Write("Select number: ");

            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/NISS/GenFAReport_Departure_FVMS_To_ISS");
                    break;
                case 2:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/NISS/GenFAReport_Daily_FVMS_To_ISS");
                    break;
                case 3:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/NISS/GenFAReport_ReportPriorNotifOfArrivalPortLanding_FVMS_To_ISS");
                    break;
                case 4:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/NISS/GenFAReport_LandingDeclaration_FVMS_To_ISS");
                    break;
            }
        }

        public static async Task FluxVesselTests(this ISecureHttpClient secureHttpClient)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("1. GenVesselQuery_FLUX_To_ISS");
            builder.AppendLine("2. GenVesselResponseErr_FLUX_To_ISS");
            builder.AppendLine("3. GenVesselQuery_ISS_To_FLUX");

            Console.Write(builder.ToString());
            Console.WriteLine();
            Console.Write("Select number: ");

            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    await secureHttpClient.SendVesselReport("http://212.72.201.242:5557/api/VesselDomain/GenVesselQuery_FLUX_To_ISS");
                    break;
                case 2:
                    await secureHttpClient.SendVesselReport("http://212.72.201.242:5557/api/VesselDomain/GenVesselResponseErr_FLUX_To_ISS");
                    break;
                case 3:
                    await secureHttpClient.SendVesselReport("http://212.72.201.242:5557/api/VesselDomain/GenVesselQuery_ISS_To_FLUX");
                    break;
            }
        }

        public static async Task FluxFishingActivityTests(this ISecureHttpClient secureHttpClient)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("1. SubFADoc");
            builder.AppendLine("2. QueryDoc");
            builder.AppendLine("3. GenFAQuery_ISS_To_FLUX");

            Console.Write(builder.ToString());
            Console.WriteLine();
            Console.Write("Select number: ");

            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/FADomain/SubFADoc");
                    break;
                case 2:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/FADomain/QueryDoc");
                    break;
                case 3:
                    await secureHttpClient.SendFishingActivities("http://212.72.201.242:5557/api/FADomain/GenFAQuery_ISS_To_FLUX");
                    break;
            }
        }

        public static async Task IaraEndpointTests(this ISecureHttpClient secureHttpClient)
        {
            //Vessels
            await secureHttpClient.MakeFluxVesselQuery("/api/Integration/VesselDomain/FluxVesselQuery");
            await secureHttpClient.UpdateResponseStatus("/api/Integration/VesselDomain/UpdateReportStatus");
            await secureHttpClient.VesselQueryReply("/api/Integration/VesselDomain/VesselQueryReply");

            //Fishing Activities
            await secureHttpClient.PublishFAReport("/api/Integration/FishingActivities/PublishFAReport");
            await secureHttpClient.FAQuery("/api/Integration/FishingActivities/FAQuery");
        }

    }
}
