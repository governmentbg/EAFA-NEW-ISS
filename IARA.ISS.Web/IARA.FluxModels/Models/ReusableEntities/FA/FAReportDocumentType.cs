using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FAReportDocumentType
    {
        /// <summary>
        /// Type - FLUX_FA_REPORT_TYPE
        /// <ram:TypeCode listID="FLUX_FA_REPORT_TYPE">DECLARATION</ram:TypeCode>
        /// </summary>
        public CodeType TypeCode { get; set; }

        /// <summary>
        /// FMC_Marker - FLUX_FA_FMC - optional
        /// </summary>
        public CodeType FMCMarkerCode { get; set; }

        /// <summary>
        /// Related_Report Identification - optional
        /// </summary>
        [XmlElement("RelatedReportID")]
        public IDType[] RelatedReportID { get; set; }

        /// <summary>
        /// Acceptance - DateTime - mandatory
        /// <ram:AcceptanceDateTime>
        ///     <udt:DateTime>2020-05-06T13:39:33.176Z</udt:DateTime>
        /// </ram:AcceptanceDateTime>
        /// </summary>
        public DateTimeType AcceptanceDateTime { get; set; }

        /// <summary>
        /// RelatedFluxReport_Document - mandatory 
        /// </summary>
        public FLUXReportDocumentType RelatedFLUXReportDocument { get; set; }

        /// <summary>
        /// SpecifiedFishing_Activity
        /// Departure etc.
        /// </summary>
        [XmlElement("SpecifiedFishingActivity")]
        public FishingActivityType[] SpecifiedFishingActivity { get; set; }

        /// <summary>
        /// SpecifiedVessel_TransportMeans - optional in case of deletion or cancellation report, otherwise mandatory
        /// </summary>
        public VesselTransportMeansType SpecifiedVesselTransportMeans { get; set; }
    }
}
