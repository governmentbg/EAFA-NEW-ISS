using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{

    #region FA Domain
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {

        private EnvelopeBody bodyField;


        public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {

        private POSTMSG pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSG POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSG
    {

        private FLUXFAReportMessageType fLUXFAReportMessageField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAReportMessage:3")]
        public FLUXFAReportMessageType FLUXFAReportMessage
        {
            get
            {
                return this.fLUXFAReportMessageField;
            }
            set
            {
                this.fLUXFAReportMessageField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }

    #region FA Query to FLUX
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeFAQuery
    {

        private EnvelopeBodyFAQuery bodyField;


        public EnvelopeBodyFAQuery Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyFAQuery
    {

        private POSTMSGFAQuery pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGFAQuery POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGFAQuery
    {

        private FLUXFAQueryMessageType FLUXFAQueryMessageTypeField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFAQueryMessage:5")]
        public FLUXFAQueryMessageType FLUXFAQueryMessage
        {
            get
            {
                return this.FLUXFAQueryMessageTypeField;
            }
            set
            {
                this.FLUXFAQueryMessageTypeField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion
    #endregion

    #region MDM


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeMDM
    {

        private EnvelopeBodyMDM bodyField;


        public EnvelopeBodyMDM Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyMDM
    {

        private POSTMSGMDM pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGMDM POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGMDM
    {

        private FLUXMDRQueryMessageType fLUXMDRQueryMessageTypeField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXMDRQueryMessage:5")]
        public FLUXMDRQueryMessageType FLUXMDRQueryMessage
        {
            get
            {
                return this.fLUXMDRQueryMessageTypeField;
            }
            set
            {
                this.fLUXMDRQueryMessageTypeField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion

    #region Vessel Domain
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeVes
    {

        private EnvelopeBodyVes bodyField;


        public EnvelopeBodyVes Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyVes
    {

        private POSTMSGVes pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGVes POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGVes
    {

        private FLUXReportVesselInformationType FLUXReportVesselInformationTypeField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXReportVesselInformation:5")]
        public FLUXReportVesselInformationType FLUXReportVesselInformation
        {
            get
            {
                return this.FLUXReportVesselInformationTypeField;
            }
            set
            {
                this.FLUXReportVesselInformationTypeField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }

    #region Vessel Query
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeVesQuery
    {

        private EnvelopeBodyVesQuery bodyField;


        public EnvelopeBodyVesQuery Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyVesQuery
    {

        private POSTMSGVesQuery pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGVesQuery POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGVesQuery
    {

        private FLUXVesselQueryMessageType FLUXVesselQueryMessageTypeField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselQueryMessage:5")]
        public FLUXVesselQueryMessageType FLUXVesselQueryMessage
        {
            get
            {
                return this.FLUXVesselQueryMessageTypeField;
            }
            set
            {
                this.FLUXVesselQueryMessageTypeField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion
    #endregion

    #region Sales Domain
    #region Sales Query
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeSalesQuery
    {

        private EnvelopeBodySalesQuery bodyField;


        public EnvelopeBodySalesQuery Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodySalesQuery
    {

        private POSTMSGSalesQuery pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGSalesQuery POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGSalesQuery
    {

        private FLUXSalesQueryMessageType FLUXSalesQueryMessageTypeField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesQueryMessage:5")]
        public FLUXSalesQueryMessageType FLUXSalesQueryMessage
        {
            get
            {
                return this.FLUXSalesQueryMessageTypeField;
            }
            set
            {
                this.FLUXSalesQueryMessageTypeField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion

    #region Sales Submission
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeSales
    {

        private EnvelopeBodySales bodyField;


        public EnvelopeBodySales Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodySales
    {

        private POSTMSGSales pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGSales POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGSales
    {

        private FLUXSalesReportMessageType FLUXSalesReportMessageField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXSalesReportMessage:5")]
        public FLUXSalesReportMessageType FLUXSalesReportMessage
        {
            get
            {
                return this.FLUXSalesReportMessageField;
            }
            set
            {
                this.FLUXSalesReportMessageField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion
    #endregion

    #region FluxResponseMessageType
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeFluxResponse
    {

        private EnvelopeBodyFluxResponse bodyField;


        public EnvelopeBodyFluxResponse Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyFluxResponse
    {

        private POSTMSGFluxResponse pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGFluxResponse POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGFluxResponse
    {

        private FLUXResponseMessageType FLUXResponseMessageTypeField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXResponseMessage:5")]
        public FLUXResponseMessageType FLUXResponseMessage
        {
            get
            {
                return this.FLUXResponseMessageTypeField;
            }
            set
            {
                this.FLUXResponseMessageTypeField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion

    #region FLAP
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeFLAPRequest
    {

        private EnvelopeBodyFLAPRequest bodyField;


        public EnvelopeBodyFLAPRequest Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyFLAPRequest
    {

        private POSTMSGFLAPRequest pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGFLAPRequest POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGFLAPRequest
    {

        private FLUXFLAPRequestMessageType FLUXFLAPRequestMessageField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPRequestMessage:5")]
        public FLUXFLAPRequestMessageType FLUXFLAPRequestMessage
        {
            get
            {
                return this.FLUXFLAPRequestMessageField;
            }
            set
            {
                this.FLUXFLAPRequestMessageField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion

    #region ACDR
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeMDR
    {

        private EnvelopeBodyMDR bodyField;


        public EnvelopeBodyMDR Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyMDR
    {

        private POSTMSGMDR pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGMDR POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGMDR
    {

        private FLUXMDRQueryMessageType FLUXMDRQueryMessageField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXMDRQueryMessage:5")]
        public FLUXMDRQueryMessageType FLUXMDRQueryMessage
        {
            get
            {
                return this.FLUXMDRQueryMessageField;
            }
            set
            {
                this.FLUXMDRQueryMessageField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion

    #region ACDR
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", TypeName = "Envelope")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class EnvelopeACDR
    {

        private EnvelopeBodyACDR bodyField;


        public EnvelopeBodyACDR Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBodyACDR
    {

        private POSTMSGACDR pOSTMSGField;


        [XmlElement(Namespace = "urn:xeu:connector-bridge:v1")]
        public POSTMSGACDR POSTMSG
        {
            get
            {
                return this.pOSTMSGField;
            }
            set
            {
                this.pOSTMSGField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGACDR
    {

        private FLUXACDRMessageType FLUXACDRMessageField;

        private string dtField;

        private string adField;

        private bool arField;

        private string dfField;

        private string frField;

        private bool tsField;

        private string vbField;


        [XmlElement(Namespace = "urn:un:unece:uncefact:data:standard:FLUXACDRMessage:5")]
        public FLUXACDRMessageType FLUXACDRMessage
        {
            get
            {
                return this.FLUXACDRMessageField;
            }
            set
            {
                this.FLUXACDRMessageField = value;
            }
        }


        //[XmlAttribute]
        [XmlIgnore]
        public string DT
        {
            get
            {
                return this.dtField;
            }
            set
            {
                this.dtField = value;
            }
        }


        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public bool AR
        {
            get
            {
                return this.arField;
            }
            set
            {
                this.arField = value;
            }
        }


        [XmlAttribute]
        public string DF
        {
            get
            {
                return this.dfField;
            }
            set
            {
                this.dfField = value;
            }
        }


        [XmlAttribute]
        public string FR
        {
            get
            {
                return this.frField;
            }
            set
            {
                this.frField = value;
            }
        }


        [XmlAttribute]
        public bool TS
        {
            get
            {
                return this.tsField;
            }
            set
            {
                this.tsField = value;
            }
        }


        [XmlAttribute]
        public string VB
        {
            get
            {
                return this.vbField;
            }
            set
            {
                this.vbField = value;
            }
        }
    }
    #endregion

    ////////////////////////////////////////////////
    ///

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    [XmlRoot(Namespace = "urn:xeu:connector-bridge:v1", IsNullable = false)]
    public partial class POSTMSGOUT
    {

        private POSTMSGOUTAssignedON assignedONField;

        /// <summary>
        /// PK, ignored in XML Serialization
        /// </summary>
        [XmlIgnoreAttribute]
        [Key]
        public long Id { get; set; }

        public POSTMSGOUTAssignedON AssignedON
        {
            get
            {
                return this.assignedONField;
            }
            set
            {
                this.assignedONField = value;
            }
        }
    }


    [Serializable]

    [XmlType(AnonymousType = true, Namespace = "urn:xeu:connector-bridge:v1")]
    public partial class POSTMSGOUTAssignedON
    {

        private string adField;

        private string onField;


        [Key]
        public long Id { get; set; }



        [XmlAttribute]
        public string AD
        {
            get
            {
                return this.adField;
            }
            set
            {
                this.adField = value;
            }
        }


        [XmlAttribute]
        public string ON
        {
            get
            {
                return this.onField;
            }
            set
            {
                this.onField = value;
            }
        }
    }

    //////////////
    ///






}
