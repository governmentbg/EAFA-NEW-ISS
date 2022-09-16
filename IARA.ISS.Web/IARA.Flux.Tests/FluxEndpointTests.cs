using System;
using System.Threading.Tasks;
using IARA.Flux.Models;
using TL.SysToSysSecCom;

namespace IARA.Flux.Tests
{
    internal static class FluxEndpointTests
    {
        public static async Task SendVesselReport(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync(requestUrl, new FLUXReportVesselInformationType
                {
                    FLUXReportDocument = new FLUXReportDocumentType
                    {
                        CreationDateTime = DateTime.Now,
                        ID = new IDType[] { new IDType { schemeID = "gaega", Value = "gaega" } },
                        TypeCode = new CodeType { listID = "gaega", Value = "gaegea" }
                    },
                    VesselEvent = new VesselEventType[]
                    {
                        new VesselEventType
                        {
                            ID = new IDType[] { new IDType { schemeID = "gegae", Value = "gaegea" } },
                            Description = new TextType[] { (TextType)"gaegeg" }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public static async Task SendFishingActivities(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync<long>(requestUrl, 3);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

    }
}
