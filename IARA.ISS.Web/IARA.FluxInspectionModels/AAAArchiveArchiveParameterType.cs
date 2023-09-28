namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveArchiveParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveArchiveParameterType
    {

        private DateTimeType inputDateTimeField;

        private DateTimeType scheduledDestructionDateTimeField;

        private DateTimeType prescriptionDateTimeField;

        private IDType destructionAuthorizationLevelIDField;

        private AAAArchivePartyType inputResponsibleAAAArchivePartyField;

        private AAAArchivePartyType destructionResponsibleAAAArchivePartyField;

        private AAAArchiveAuthenticationType trustedThirdPartyAAAArchiveAuthenticationField;

        private AAAArchiveAuthenticationType agentAAAArchiveAuthenticationField;

        public DateTimeType InputDateTime
        {
            get
            {
                return this.inputDateTimeField;
            }
            set
            {
                this.inputDateTimeField = value;
            }
        }

        public DateTimeType ScheduledDestructionDateTime
        {
            get
            {
                return this.scheduledDestructionDateTimeField;
            }
            set
            {
                this.scheduledDestructionDateTimeField = value;
            }
        }

        public DateTimeType PrescriptionDateTime
        {
            get
            {
                return this.prescriptionDateTimeField;
            }
            set
            {
                this.prescriptionDateTimeField = value;
            }
        }

        public IDType DestructionAuthorizationLevelID
        {
            get
            {
                return this.destructionAuthorizationLevelIDField;
            }
            set
            {
                this.destructionAuthorizationLevelIDField = value;
            }
        }

        public AAAArchivePartyType InputResponsibleAAAArchiveParty
        {
            get
            {
                return this.inputResponsibleAAAArchivePartyField;
            }
            set
            {
                this.inputResponsibleAAAArchivePartyField = value;
            }
        }

        public AAAArchivePartyType DestructionResponsibleAAAArchiveParty
        {
            get
            {
                return this.destructionResponsibleAAAArchivePartyField;
            }
            set
            {
                this.destructionResponsibleAAAArchivePartyField = value;
            }
        }

        public AAAArchiveAuthenticationType TrustedThirdPartyAAAArchiveAuthentication
        {
            get
            {
                return this.trustedThirdPartyAAAArchiveAuthenticationField;
            }
            set
            {
                this.trustedThirdPartyAAAArchiveAuthenticationField = value;
            }
        }

        public AAAArchiveAuthenticationType AgentAAAArchiveAuthentication
        {
            get
            {
                return this.agentAAAArchiveAuthenticationField;
            }
            set
            {
                this.agentAAAArchiveAuthenticationField = value;
            }
        }
    }
}