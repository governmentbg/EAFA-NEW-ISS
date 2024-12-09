using IARA.FluxInspectionModels;
using IARA.FluxModels;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain.Helpers
{
    public class FluxInspectionModelsHelper
    {
        public static Flux.Models.IDType FindReportDocumentId(FLUXReportDocumentType document)
        {
            IDType id = document.ID?.Where(x => x.schemeID == IDTypes.UUID).FirstOrDefault();
            return FindIDType(id);
        }

        public static Flux.Models.IDType FindResponseDocumentId(FLUXResponseDocumentType document)
        {
            IDType id = document.ID?.Where(x => x.schemeID == IDTypes.UUID).FirstOrDefault();
            return FindIDType(id);
        }

        public static Flux.Models.IDType FindIDType(IDType id)
        {
            if (id == null)
            {
                return null;
            }

            return new Flux.Models.IDType
            {
                schemeID = id.schemeID,
                schemeName = id.schemeName,
                schemeAgencyID = id.schemeAgencyID,
                schemeAgencyName = id.schemeAgencyName,
                schemeVersionID = id.schemeVersionID,
                schemeDataURI = id.schemeDataURI,
                schemeURI = id.schemeURI,
                Value = id.Value
            };
        }

        public static Flux.Models.CodeType FindCodeType(CodeType code)
        {
            if (code == null)
            {
                return null;
            }

            return new Flux.Models.CodeType
            {
                languageID = code.languageID,
                listAgencyID = code.listAgencyID,
                listAgencyName = code.listAgencyName,
                listID = code.listID,
                listName = code.listName,
                listSchemeURI = code.listSchemeURI,
                listURI = code.listURI,
                listVersionID = code.listVersionID,
                name = code.name,
                Value = code.Value
            };
        }

        public static Flux.Models.FLUXResponseDocumentType FindResponseDocumentType(FLUXResponseDocumentType document)
        {
            if (document == null)
            {
                return null;
            }

            Flux.Models.FLUXResponseDocumentType result = new Flux.Models.FLUXResponseDocumentType
            {
                ReferencedID = FindIDType(document.ReferencedID),
                ResponseCode = FindCodeType(document.ResponseCode),
                // RelatedValidationResultDocument = document.RelatedValidationResultDocument, //TODO?
            };

            List<Flux.Models.IDType> documentIds = new List<Flux.Models.IDType>();

            foreach (IDType id in document.ID)
            {
                documentIds.Add(FindIDType(id));
            }

            result.ID = documentIds.ToArray();

            return result;
        }

        public static Flux.Models.FLUXResponseMessageType FindResponseMessageType(FLUXISRResponseMessageType message)
        {
            if (message == null)
            {
                return null;
            }

            Flux.Models.FLUXResponseMessageType result = new Flux.Models.FLUXResponseMessageType
            {
                FLUXResponseDocument = FindResponseDocumentType(message.FLUXResponseDocument)
            };

            return result;
        }
    }
}
