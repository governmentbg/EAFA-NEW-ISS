using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Serialization;

namespace TechnoLogica.Authentication.Common
{
    // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.xml.signedxml.checksignature?view=netframework-4.7.2
    public class SigningUtils
    {
        public const string SignatureXPath = "//*[local-name()='Signature']";

        /// <summary>
        /// Get certificate from certificate store
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="findType"></param>
        /// <param name="findValue"></param>
        /// <returns></returns>
        public static X509Certificate2 GetX509Certificate(StoreName storeName, StoreLocation storeLocation, X509FindType findType, object findValue, bool throwExceptionIfNone = true)
        {
            // Try to open the store.
            X509Store certStore = new(storeName, storeLocation);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection;
            try
            {
                // Find the certificate that matches the thumbprint.
                certCollection = certStore.Certificates.Find(findType, findValue, validOnly: false);
            }
            catch (Exception)
            {
                certStore.Close();
                certStore.Dispose();

                throw;
            }

            if (throwExceptionIfNone)
            {
                // Check to see if our certificate was added to the collection.
                if (certCollection.Count == 0)
                {
                    string message = string.Format("Certificate \"{0}\" not found in {1}, {2}.", findValue, storeName.ToString(), storeLocation.ToString());
                    throw new ArgumentException(message);
                }

                return certCollection[0];
            }

            return certCollection.Count == 0 ? null : certCollection[0];
        }

        public static bool HasSignature(XmlDocument doc)
        {
            XmlNodeList signatureNodes = doc.SelectNodes(SignatureXPath);
            return signatureNodes.Count > 0;
        }

        public static bool ValidateXmlDocumentSignature(XmlElement doc)
        {
            try
            {
                SignedXml signedXml = new(doc);
                XmlNode signatureNode = doc.SelectNodes(SignatureXPath)[0];
                signedXml.LoadXml((XmlElement)signatureNode);
                bool result = signedXml.CheckSignature();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ValidateXmlDocumentSignature(XmlElement doc, X509Certificate2 cert)
        {
            try
            {
                SignedXml signedXml = new(doc);
                XmlNode signatureNode = doc.SelectNodes(SignatureXPath)[0];
                signedXml.LoadXml((XmlElement)signatureNode);
                // See https://github.com/microsoft/dotnet/issues/341
                bool result = signedXml.CheckSignature(cert.PublicKey.Key);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Computes signature based on X509Certificate2 and sign the xml with private key
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cert"></param
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public static XmlElement CreateSignatureWithCertificate(XmlElement doc, X509Certificate2 cert, string signatureMethod = SignedXml.XmlDsigRSASHA1Url)
        {
            if (!cert.HasPrivateKey)
            {
                // TODO: business exception
                throw new ArgumentException("The selected certificate doesn't have private key");
            }

            // Create a SignedXml object.
            SignedXml signedXml = new(doc)
            {
                SigningKey = cert.PrivateKey
            };

            // Create a reference to be signed.
            Reference reference = new()
            {
                Uri = ""
            };

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new();
            reference.AddTransform(env);
            signedXml.AddReference(reference);

            // Add KeyInfo - optional, helps recipient find key to validate.
            KeyInfo keyInfo = new();
            KeyInfoX509Data keyInfoClause = new(cert);
            keyInfoClause.AddIssuerSerial(cert.IssuerName?.Name, cert.SerialNumber);
            keyInfoClause.AddSubjectName(cert.SubjectName?.Name);
            keyInfo.AddClause(keyInfoClause);
            signedXml.KeyInfo = keyInfo;
            signedXml.SignedInfo.SignatureMethod = signatureMethod;

            // Compute signature
            signedXml.ComputeSignature();
            XmlElement xmlSig = signedXml.GetXml();
            return xmlSig;
        }

        /// <summary>
        /// Computes signature based on X509Certificate2 and sign the xml with private key
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cert"></param>
        /// <param name="signatureMethod"></param>
        /// <returns></returns>
        public static XmlElement CreateEAuthSignature<T>(T obj, X509Certificate2 cert, XmlSerializerNamespaces ns = null, string signatureMethod = SignedXml.XmlDsigRSASHA1Url)
        {
            string dataXml = XmlUtils.SerializeToXml(obj, ns);
            XmlElement dataElement = XmlUtils.ToXmlElement(dataXml);
            XmlElement signature = CreateSignatureWithCertificate(dataElement, cert, signatureMethod);

            XmlDocument document = new()
            {
                PreserveWhitespace = false
            };
            document.LoadXml(dataXml);

            // Removes xml tag
            _ = document.RemoveChild(document.ChildNodes[0]);

            // Attach signature
            XmlElement rootElement = document.DocumentElement;
            XmlNode importNode = document.ImportNode(signature, true);

            // Insert on second place for xsd validation
            _ = rootElement.InsertAfter(importNode, rootElement.FirstChild);

            return rootElement;
        }

        private static string ValidateCertificateChain(X509Certificate2 cert)
        {
            List<string> result = new();
            using (X509Chain chain = new())
            {
                // The defaults, but expressing it here for clarity
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                chain.ChainPolicy.VerificationTime = DateTime.Now;

                _ = chain.Build(cert);

                for (int i = 0; i < chain.ChainElements.Count; i++)
                {
                    X509ChainElement element = chain.ChainElements[i];

                    if (element.ChainElementStatus.Length != 0)
                    {
                        string message = $"Error at depth {i}: {element.Certificate.Subject}";
                        result.Add(message);

                        foreach (X509ChainStatus status in element.ChainElementStatus)
                        {
                            message = $"  {status.Status}: {status.StatusInformation}}}";
                            result.Add(message);
                        }
                    }
                }
            }

            string allValidations = string.Join(Environment.NewLine, result);
            return allValidations;
        }
    }
}
