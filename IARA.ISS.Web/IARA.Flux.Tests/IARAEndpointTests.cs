using System;
using System.Threading.Tasks;
using IARA.Flux.Models;
using TL.SysToSysSecCom;

namespace IARA.Flux.Tests
{
    internal static class IARAEndpointTests
    {
        public static async Task FAQuery(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync(requestUrl, new FLUXFAQueryMessageType
                {
                    FAQuery = new FAQueryType
                    {
                        ID = new IDType { schemeID = "geaga", Value = "gaegae" },
                        SubmittedDateTime = DateTime.Now,
                        TypeCode = new CodeType { listID = "gega", Value = "gaegea" }
                    }
                });

                Console.WriteLine($"Метод: {nameof(FAQuery)} успешно извикан");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public static async Task PublishFAReport(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync(requestUrl, new FLUXFAReportMessageType
                {
                    FLUXReportDocument = new FLUXReportDocumentType
                    {
                        CreationDateTime = DateTime.Now,
                        ID = new IDType[] { new IDType { schemeID = "524524", Value = "гаегае" } }
                    },
                    FAReportDocument = new FAReportDocumentType[]
                    {
                        new FAReportDocumentType
                        {
                            AcceptanceDateTime = DateTime.Now,
                            RelatedReportID = new IDType[] { new IDType { schemeID = "gegae", Value = "gaegae" } },
                            TypeCode = new CodeType { listID = "geaga", Value = "gaegae" }
                        }
                    }
                });

                Console.WriteLine($"Метод: {nameof(PublishFAReport)} успешно извикан");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public static async Task VesselQueryReply(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync(requestUrl, new FLUXVesselResponseMessageType
                {
                    FLUXResponseDocument = new FLUXResponseDocumentType
                    {
                        CreationDateTime = DateTime.Now,
                        ID = new IDType[] { new IDType { schemeID = "UUID", Value = Guid.NewGuid().ToString() } },
                        RejectionReason = (TextType)"gaegaed",
                        Remarks = (TextType)"test test test",
                        TypeCode = new CodeType { listID = "gega", Value = "gaegae" },
                        RelatedValidationResultDocument = new ValidationResultDocumentType[] { new ValidationResultDocumentType { CreationDateTime = DateTime.Now } }
                    },
                    EventVesselEvent = new VesselEventType[]
                    {
                        new VesselEventType
                        {
                            Description = new TextType[] { (TextType)"gaegaegea" },
                            VesselID = new IDType { schemeID = "geaga", Value = "gaegae" }
                        }
                    }
                });

                Console.WriteLine($"Метод: {nameof(VesselQueryReply)} успешно извикан");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public static async Task UpdateResponseStatus(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync<FLUXResponseMessageType>(requestUrl, new FLUXResponseMessageType
                {
                    FLUXResponseDocument = new FLUXResponseDocumentType
                    {
                        CreationDateTime = DateTime.Now,
                        ID = new IDType[] { new IDType { schemeID = "UUID", Value = Guid.NewGuid().ToString() } },
                        RejectionReason = (TextType)"gaegaed",
                        Remarks = (TextType)"test test test",
                        TypeCode = new CodeType { listID = "gega", Value = "gaegae" },
                        RelatedValidationResultDocument = new ValidationResultDocumentType[] { new ValidationResultDocumentType { CreationDateTime = DateTime.Now } }
                    }
                });

                Console.WriteLine($"Метод: {nameof(UpdateResponseStatus)} успешно извикан");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public static async Task MakeFluxVesselQuery(this ISecureHttpClient secureHttpClient, string requestUrl)
        {
            try
            {
                await secureHttpClient.SendAsync(requestUrl, new FLUXVesselQueryMessageType
                {
                    VesselQuery = new VesselQueryType
                    {
                        ID = new IDType
                        {

                            schemeID = "UUID",
                            Value = Guid.NewGuid().ToString()
                        },
                        TypeCode = new CodeType
                        {
                            listID = "",
                            Value = "gaegaeg"
                        },
                        SubmittedDateTime = DateTime.Now,
                        SubjectVesselIdentity = new VesselIdentityType
                        {
                            VesselID = new IDType[] { new IDType { schemeID = "gaegaegae", Value = "gaegae" } }
                        },
                        SpecifiedDelimitedPeriod = new DelimitedPeriodType { StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddDays(34) },
                        SubmitterFLUXParty = new FLUXPartyType
                        {
                            ID = new IDType[] { new IDType { schemeID = "gaegaeg", Value = "gaegegea" } },
                            Name = new TextType[] { new TextType { languageID = "BG", Value = "geagaeg" } }
                        }
                    }
                });

                Console.WriteLine($"Метод: {nameof(MakeFluxVesselQuery)} успешно извикан");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }


    }
}
