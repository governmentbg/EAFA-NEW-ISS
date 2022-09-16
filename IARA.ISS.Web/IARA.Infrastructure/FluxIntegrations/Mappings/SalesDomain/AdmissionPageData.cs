using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain.Base;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain
{
    internal class AdmissionPageData : FirstSaleAdmissionPageData
    {
        private string acceptorId;
        private string acceptorName;

        public static AdmissionPageData GetPageData(int id, IARADbContext db)
        {
            AdmissionPageData result = (from page in db.AdmissionLogBookPages
                                        // Ship logbook
                                        join originDeclaration in db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                                        join shipPage in db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                        join shipLogbook in db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                                        join logBookPermitLicense in db.LogBookPermitLicenses on shipLogbook.Id equals logBookPermitLicense.LogBookId 
                                        join permitLicense in db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                        join captain in db.FishermenRegisters on permitLicense.QualifiedFisherId equals captain.Id
                                        join captainPerson in db.Persons on captain.PersonId equals captainPerson.Id
                                        // Vessel data
                                        join ship in db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                                        join shipFlagCountry in db.Ncountries on ship.FlagCountryId equals shipFlagCountry.Id
                                        join landingPort in db.Nports on shipPage.ArrivePortId equals landingPort.Id
                                        // Logbook
                                        join logbook in db.LogBooks on page.LogBookId equals logbook.Id
                                        join buyer in db.BuyerRegisters on logbook.RegisteredBuyerId equals buyer.Id into buy
                                        from buyer in buy.DefaultIfEmpty()
                                        join buyerLegal in db.Legals on buyer.SubmittedForLegalId equals buyerLegal.Id into bl
                                        from buyerLegal in bl.DefaultIfEmpty()
                                        join person in db.Persons on logbook.PersonId equals person.Id into per
                                        from person in per.DefaultIfEmpty()
                                        join legal in db.Legals on logbook.LegalId equals legal.Id into leg
                                        from legal in leg.DefaultIfEmpty()
                                        where page.Id == id
                                        select new AdmissionPageData
                                        {
                                            salesType = FluxSalesTypes.TOD,
                                            date = page.HandoverDate.Value,
                                            pageNumber = page.PageNum.ToString(),
                                            location = page.StorageLocation,
                                            landingPortCode = landingPort.Code,
                                            landingDateTime = shipPage.FishTripEndDateTime.Value,
                                            shipCfr = ship.Cfr,
                                            shipName = ship.Name,
                                            shipFlagCountryCode = shipFlagCountry.Code,
                                            acceptorId = buyerLegal != null 
                                                ? buyerLegal.Eik
                                                : person != null 
                                                    ? person.EgnLnc
                                                    : legal != null 
                                                        ? legal.Eik
                                                        : null,
                                            acceptorName = buyerLegal != null 
                                                ? buyerLegal.Name
                                                : person != null 
                                                    ? $"{person.FirstName} {person.LastName}"
                                                    : legal != null 
                                                        ? legal.Name
                                                        : null,
                                            captainEgn = captainPerson.EgnLnc,
                                            captainFirstName = captainPerson.FirstName,
                                            captainMiddleName = captainPerson.MiddleName,
                                            captainLastName = captainPerson.LastName
                                        }).First();

            result.tripIdentifier = GetTripIdentifier(db, id);
            result.products = ProductData.GetAdmissionProductData(db, id);

            return result;
        }

        protected override SalesPartyType[] CreateSalesParties()
        {
            // Mandatory: SENDER, RECIPIENT
            SalesPartyType[] parties = new SalesPartyType[]
            {
                new SalesPartyType
                {
                    ID = IDType.CreateID(FluxSalesPartyIdTypes.MS, acceptorId),
                    Name = TextType.CreateText(acceptorName),
                    RoleCode = new CodeType[]{ CodeType.CreateCode(ListIDTypes.FLUX_SALES_PARTY_ROLE, FluxSalesPartyRoles.SENDER) }
                },
                new SalesPartyType
                {
                    ID = IDType.CreateID(FluxSalesPartyIdTypes.MS, acceptorId),
                    Name = TextType.CreateText(acceptorName),
                    RoleCode = new CodeType[]{ CodeType.CreateCode(ListIDTypes.FLUX_SALES_PARTY_ROLE, FluxSalesPartyRoles.RECIPIENT) }
                }
            };

            return parties.ToArray();
        }

        private static string GetTripIdentifier(IARADbContext db, int pageId)
        {
            string id = (from page in db.AdmissionLogBookPages
                         join originDeclaration in db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                         join shipPage in db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                         join fa in db.FvmsfishingActivityReportLogBookPages on shipPage.Id equals fa.ShipLogBookPageId
                         where page.Id == pageId
                         select fa.TripIdentifier).First();

            return id;
        }
    }
}
