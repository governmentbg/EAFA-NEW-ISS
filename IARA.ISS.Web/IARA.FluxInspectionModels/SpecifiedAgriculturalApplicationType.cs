namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedAgriculturalApplication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedAgriculturalApplicationType
    {

        private IDType idField;

        private ReferencedLocationType[] specifiedReferencedLocationField;

        private AgriculturalZoneAreaType[] appliedAgriculturalZoneAreaField;

        private ProductBatchType[] specifiedProductBatchField;

        private AgriculturalCertificateType[] appliedAgriculturalCertificateField;

        private CropPlotType[] specifiedCropPlotField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [XmlElement("SpecifiedReferencedLocation")]
        public ReferencedLocationType[] SpecifiedReferencedLocation
        {
            get
            {
                return this.specifiedReferencedLocationField;
            }
            set
            {
                this.specifiedReferencedLocationField = value;
            }
        }

        [XmlElement("AppliedAgriculturalZoneArea")]
        public AgriculturalZoneAreaType[] AppliedAgriculturalZoneArea
        {
            get
            {
                return this.appliedAgriculturalZoneAreaField;
            }
            set
            {
                this.appliedAgriculturalZoneAreaField = value;
            }
        }

        [XmlElement("SpecifiedProductBatch")]
        public ProductBatchType[] SpecifiedProductBatch
        {
            get
            {
                return this.specifiedProductBatchField;
            }
            set
            {
                this.specifiedProductBatchField = value;
            }
        }

        [XmlElement("AppliedAgriculturalCertificate")]
        public AgriculturalCertificateType[] AppliedAgriculturalCertificate
        {
            get
            {
                return this.appliedAgriculturalCertificateField;
            }
            set
            {
                this.appliedAgriculturalCertificateField = value;
            }
        }

        [XmlElement("SpecifiedCropPlot")]
        public CropPlotType[] SpecifiedCropPlot
        {
            get
            {
                return this.specifiedCropPlotField;
            }
            set
            {
                this.specifiedCropPlotField = value;
            }
        }
    }
}