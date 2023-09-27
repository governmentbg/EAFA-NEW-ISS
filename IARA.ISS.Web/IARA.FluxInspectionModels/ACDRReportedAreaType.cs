namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ACDRReportedArea", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ACDRReportedAreaType
    {

        private CodeType fAOIdentificationCodeField;

        private CodeType sovereigntyWaterCodeField;

        private CodeType landingCountryCodeField;

        private CodeType catchStatusCodeField;

        private ACDRCatchType[] specifiedACDRCatchField;

        public CodeType FAOIdentificationCode
        {
            get
            {
                return this.fAOIdentificationCodeField;
            }
            set
            {
                this.fAOIdentificationCodeField = value;
            }
        }

        public CodeType SovereigntyWaterCode
        {
            get
            {
                return this.sovereigntyWaterCodeField;
            }
            set
            {
                this.sovereigntyWaterCodeField = value;
            }
        }

        public CodeType LandingCountryCode
        {
            get
            {
                return this.landingCountryCodeField;
            }
            set
            {
                this.landingCountryCodeField = value;
            }
        }

        public CodeType CatchStatusCode
        {
            get
            {
                return this.catchStatusCodeField;
            }
            set
            {
                this.catchStatusCodeField = value;
            }
        }

        [XmlElement("SpecifiedACDRCatch")]
        public ACDRCatchType[] SpecifiedACDRCatch
        {
            get
            {
                return this.specifiedACDRCatchField;
            }
            set
            {
                this.specifiedACDRCatchField = value;
            }
        }
    }
}