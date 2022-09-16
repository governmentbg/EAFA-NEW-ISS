using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.Interfaces.Reports;
using TL.JasperReports.Integration;
using TL.JasperReports.Integration.Enums;

namespace IARA.Infrastructure.Services.Reports
{
    public class JasperReportExecutionService : IJasperReportExecutionService
    {
        private readonly IJasperReportsClient jasperReportsClient;


        public JasperReportExecutionService(IJasperReportsClient jasperReportsClient)
        {
            this.jasperReportsClient = jasperReportsClient;
        }

        public Task<byte[]> GetBuyersApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Buyer_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetBuyerChangeApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Buyer_change_of_circumstances_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetBuyerDeregistrationApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Buyer_cancellation_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFirstSaleCenterChangeApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Center_first_sale_change_of_circumstances_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFirstSaleCenterDeregistrationApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Center_first_sale_cancellation_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetCapacityApplicationReduce(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Capacity_application), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetCapacityApplicationTransfer(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Capacity_application_change), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetCapacityApplicationIncrease(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Capacity_application_inscrease), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetCapacityApplicationDuplicate(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Capacity_application_duplicate), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetCommercialFishingApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Commercial_fishing_application), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetSalesCentersApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Center_first_sale_appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFishermanApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Fisherman_application), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFishingVesselsApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Fishing_vessels_application), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFishingVesselsChangeApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Fishing_vessels_application_change), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFishingVesselsDestroyApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Fishing_vessels_application_destroy), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetLegalApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Legal_appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetScientificFishingApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "id", applicationId.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Scientific_fishing_appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetAquacultureFacilityApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Aquaculture_facilities_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetAquacultureFacilityDeregApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Aquaculture_facilities_cancellation_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetAquacultureFacilityChangeApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Aquaculture_facilities_change_of_circumstances_Appl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetStatisticalFormAquacultureApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Statistical_form_aqua_farm), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetStatisticalFormFishingVesselApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Statistical_form_fish_vessel), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetStatisticalFormReworkApplication(int applicationId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", applicationId.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Statistical_form_rework), OutputFormats.pdf, inputs);
        }

        public Task<Stream> GetFishingTicket(int ticketId)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "ticket_id", ticketId.ToString() } };

            return this.jasperReportsClient.RunReport(BuildReportUrl(JasperReportsEnum.Ticket_application), OutputFormats.pdf, inputs);
        }

        public Task<Stream> GetFishingTicketDeclaration(RecreationalFishingTicketDeclarationParametersDTO parameters)
        {
            string personName;
            string egn;
            string phoneNumber;
            string email;
            string documentNum;
            string documentIssueDate;
            string documentIssuedBy;

            if (parameters.RepresentativePerson != null)
            {
                personName = $"{parameters.RepresentativePerson.FirstName}";
                if (!string.IsNullOrEmpty(parameters.RepresentativePerson.MiddleName))
                {
                    personName = $"{personName} {parameters.RepresentativePerson.MiddleName}";
                }
                personName = $"{personName} {parameters.RepresentativePerson.LastName}";

                egn = parameters.RepresentativePerson.EgnLnc.EgnLnc;
                phoneNumber = parameters.RepresentativePerson.Phone;
                email = parameters.RepresentativePerson.Email;
                documentNum = parameters.RepresentativePerson.Document.DocumentNumber;
                documentIssueDate = parameters.RepresentativePerson.Document.DocumentIssuedOn.Value.ToString();
                documentIssuedBy = parameters.RepresentativePerson.Document.DocumentIssuedBy;
            }
            else
            {
                personName = $"{parameters.Person.FirstName}";
                if (!string.IsNullOrEmpty(parameters.Person.MiddleName))
                {
                    personName = $"{personName} {parameters.Person.MiddleName}";
                }
                personName = $"{personName} {parameters.Person.LastName}";

                egn = parameters.Person.EgnLnc.EgnLnc;
                phoneNumber = parameters.Person.Phone;
                email = parameters.Person.Email;
                documentNum = parameters.Person.Document.DocumentNumber;
                documentIssueDate = parameters.Person.Document.DocumentIssuedOn.Value.Date.ToString();
                documentIssuedBy = parameters.Person.Document.DocumentIssuedBy;
            }

            List<TicketTypeEnum> membershipCardTypes = new List<TicketTypeEnum> {
                TicketTypeEnum.ASSOCIATION, TicketTypeEnum.BETWEEN14AND18ASSOCIATION, TicketTypeEnum.ELDERASSOCIATION
            };

            Dictionary<string, string> inputs = new Dictionary<string, string>
            {
                { "territory", parameters.TerritoryUnit },
                { "personName", personName },
                { "egn", egn },
                { "address", parameters.Address },
                { "phoneNumber", phoneNumber },
                { "email", email },
                { "code", parameters.Code },
                { "ticketValidFrom", parameters.ValidFrom.Value.Date.ToString() },
                { "ticketNum", parameters.TicketNum },
                { "submitDate", DateTime.Now.ToString() },
                { "documentNum", documentNum },
                { "documentIssueDate", documentIssueDate },
                { "documentIssuedBy", documentIssuedBy },
                { "PERSONALPHOTO", "Y" },
                { "MEMBERSHIPCARD", membershipCardTypes.Contains(parameters.Type.Value) ? "Y" : "N" },
                { "BIRTHCERTIFICATE", parameters.Type.Value == TicketTypeEnum.UNDER14 ? "Y" : "N" }
            };

            return this.jasperReportsClient.RunReport(BuildReportUrl(JasperReportsEnum.FishingTicketAppl), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetScientificFishingPermitRegister(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", id.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Scientific_fishing_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetScientificFishingPermitProject(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", id.ToString() } };
            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Scientific_fishing_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetScientificFishingPermitGovRegister(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "application_id", id.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Scientific_fishing_register_gov), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetScientificFishingPermitGovProject(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Scientific_fishing_register_project), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFishermanRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "permit_license_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Fisherman_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFirstSaleBuyerRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Buyer_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetFirstSaleCenterRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Cpp_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetAquacultureRegister(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Aquaculture_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetShipCapacityLicenseRegister(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Ship_capacity_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetDanubePermitRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Commercial_fishing_permit_dunav), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetBlackSeaPermitRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Commercial_fishing_permit_sea), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetPoundNetPermitRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "permitID", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.PoundNet), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetBlackSeaThirdCountryPermitRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Commercial_fishing_permit_third_country_sea), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetDanubeThirdCountryPermitRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Commercial_fishing_permit_third_country_dunav), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetPermitLicenseRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Commercial_fishing_permit_license_dunav_sea), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetPoundNetPermitLicenseRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "licenseID", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.PermitLicensePoundNet), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetQuotaSpeciesPermitLicenseRegister(int id, bool duplicate = false)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "licenseID", id.ToString() }, { "duplicate", duplicate.ToString() } };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.QuotaPermitLicense), OutputFormats.pdf, inputs);

            throw new NotImplementedException("Quota Species Special Permit");
        }

        public Task<byte[]> GetAuanRegister(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Auan_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetPenalDecreesRegister(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Penal_decrees_register), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetPenalDecreesAgreement(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Penal_decrees_agreement), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetPenalDecreesWarning(int id)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string> { { "register_id", id.ToString() } };

            return jasperReportsClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Penal_decrees_warning), OutputFormats.pdf, inputs);
        }

        public Task<byte[]> GetInspectionReport(int id, InspectionTypesEnum inspectionType)
        {
            Dictionary<string, string> inputs = new() { { "inspectionID", id.ToString() } };

            JasperReportsEnum jasperType = inspectionType switch
            {
                InspectionTypesEnum.IBS => JasperReportsEnum.ShipInspection,
                InspectionTypesEnum.IFP => JasperReportsEnum.FishermanInspections,
                InspectionTypesEnum.OFS => JasperReportsEnum.ObservationInspection,
                InspectionTypesEnum.IAQ => JasperReportsEnum.AquacultureInspection,
                InspectionTypesEnum.CWO => JasperReportsEnum.WaterObjectCheckInspection,
                InspectionTypesEnum.IBP => JasperReportsEnum.ShipPortInspection,
                InspectionTypesEnum.ITB => JasperReportsEnum.TransboardInspection,
                InspectionTypesEnum.IVH => JasperReportsEnum.TransportVehicleInspection,
                InspectionTypesEnum.IFS => JasperReportsEnum.FirstSaleInspection,
                InspectionTypesEnum.IGM => JasperReportsEnum.GearInspection,
                _ => throw new NotImplementedException("Report generation not implemented for the provided inspection type."),
            };

            return this.jasperReportsClient.RunReportBuffered(BuildReportUrl(jasperType), OutputFormats.pdf, inputs);
        }

        private static string BuildReportUrl(JasperReportsEnum reportType)
        {
            return $"{DefaultConstants.BASE_REPORTS_PATH}/{reportType}";
        }
    }
}
