using System.Collections.Generic;
using System.IO;
using TL.Signer;

namespace IARA.Fakes.InfrastructureStubs
{
    public class MockPdfSignatureValidator : IPdfSignatureValidator
    {
        public bool ValidateClientSignature(Stream inputStream, string identifier)
        {
            return true;
        }

        public bool ValidateServerSignature(Stream inputStream, string signatureName, out List<Error> errors, Dictionary<string, string> additionalMetadata = null)
        {
            errors = null;
            return true;
        }

        public bool ValidateServerSignature(byte[] inputPdfBytes, string signatureName, out List<Error> errors, Dictionary<string, string> additionalMetadata = null)
        {
            errors = null;
            return true;
        }

        public bool ValidateServerSignature<TMeta>(byte[] inputPdfBytes, string signatureName, out List<Error> errors, TMeta additionalMetadata = null) where TMeta : class
        {
            errors = null;
            return true;
        }

        public bool ValidateServerSignature<TMeta>(Stream inputStream, string signatureName, out List<Error> errors, TMeta additionalMetadata = null) where TMeta : class
        {
            errors = null;
            return true;
        }
    }
}
