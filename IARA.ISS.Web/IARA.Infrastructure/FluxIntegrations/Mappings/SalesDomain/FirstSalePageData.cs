using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain.Base;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain
{
    internal class FirstSalePageData : FirstSaleAdmissionPageData
    {
        private string buyerEik;
        private string buyerName;

        public static FirstSalePageData GetPageData(int id, IARADbContext db)
        {
            FirstSalePageData result = (from page in db.FirstSaleLogBookPages
                                        // Logbook
                                        join logbook in db.LogBooks on page.LogBookId equals logbook.Id
                                        join buyer in db.BuyerRegisters on logbook.RegisteredBuyerId equals buyer.Id
                                        join buyerLegal in db.Legals on buyer.SubmittedForLegalId equals buyerLegal.Id
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
                                        where page.Id == id
                                        select new FirstSalePageData
                                        {
                                            salesType = FluxSalesTypes.SN,
                                            pageNumber = page.PageNum.ToString(),
                                            date = page.SaleDate.Value,
                                            buyerEik = buyerLegal.Eik,
                                            buyerName = buyerLegal.Name,
                                            location = page.SaleLocation,
                                            landingPortCode = landingPort.Code,
                                            landingDateTime = shipPage.FishTripEndDateTime.Value,
                                            shipCfr = ship.Cfr,
                                            shipName = ship.Name,
                                            shipFlagCountryCode = shipFlagCountry.Code,
                                            captainEgn = captainPerson.EgnLnc,
                                            captainFirstName = captainPerson.FirstName,
                                            captainMiddleName = captainPerson.MiddleName,
                                            captainLastName = captainPerson.LastName
                                        }).First();

            result.tripIdentifier = GetTripIdentifier(db, id);
            result.products = ProductData.GetFirstSaleProductData(db, id);

            return result;
        }

        protected override SalesPartyType[] CreateSalesParties()
        {
            // Mandatory: SENDER, BUYER, PROVIDER
            SalesPartyType[] parties = new SalesPartyType[3]
            {
                new SalesPartyType
                {
                    ID = IDType.CreateID(FluxSalesPartyIdTypes.MS, buyerEik),
                    Name = TextType.CreateText(buyerName),
                    RoleCode = new CodeType[]{ CodeType.CreateCode(ListIDTypes.FLUX_SALES_PARTY_ROLE, FluxSalesPartyRoles.SENDER) }
                },
                new SalesPartyType
                {
                    ID = IDType.CreateID(FluxSalesPartyIdTypes.MS, buyerEik),
                    Name = TextType.CreateText(buyerName),
                    RoleCode = new CodeType[]{ CodeType.CreateCode(ListIDTypes.FLUX_SALES_PARTY_ROLE, FluxSalesPartyRoles.BUYER) }
                },
                new SalesPartyType
                {
                    ID = IDType.CreateID(FluxSalesPartyIdTypes.MS, captainEgn),
                    Name = TextType.CreateText($"{captainFirstName} {captainLastName}"),
                    RoleCode = new CodeType[]{ CodeType.CreateCode(ListIDTypes.FLUX_SALES_PARTY_ROLE, FluxSalesPartyRoles.PROVIDER) }
                }
            };

            return parties;
        }

        private static string GetTripIdentifier(IARADbContext db, int pageId)
        {
            string id = (from page in db.FirstSaleLogBookPages
                         join originDeclaration in db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                         join shipPage in db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                         join fa in db.FvmsfishingActivityReportLogBookPages on shipPage.Id equals fa.ShipLogBookPageId
                         where page.Id == pageId
                         select fa.TripIdentifier).First();

            return id;
        }
    }
}
