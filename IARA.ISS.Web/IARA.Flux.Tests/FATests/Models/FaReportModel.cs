using System;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaReportModel
    {
        public FluxPurposes Purpose { get; set; }

        public string PurposeText { get; set; }

        public Guid? ReferencedId { get; set; }

        public Guid[] RelatedReportIds { get; set; }

        public VesselTransportMeansType VesselTransportMeans { get; set; }

        public string FmcMarkerCode { get; set; }

        public FaReportModel(FluxPurposes purpose, 
                             string purposeText, 
                             Guid? referencedId, 
                             Guid[] relatedReportIds, 
                             VesselTransportMeansType vesselTransportMeans, 
                             string fmcMarkerCode)
        {
            this.Purpose = purpose;
            this.PurposeText = purposeText;
            this.ReferencedId = referencedId;
            this.RelatedReportIds = relatedReportIds;
            this.VesselTransportMeans = vesselTransportMeans;
            this.FmcMarkerCode = fmcMarkerCode;
        }
    }
}
