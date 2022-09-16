using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class AddressHelper
    {
        public static Address AddOrEditAddress(this IARADbContext Db, AddressRegistrationDTO address, bool deferSave = false, int? idToEdit = null)
        {
            Address addressEntry;
            if (idToEdit != null)
            {
                addressEntry = (from addressDb in Db.Addresses
                                where addressDb.Id == idToEdit
                                select addressDb).Single();
            }
            else
            {
                addressEntry = new Address();
                Db.Addresses.Add(addressEntry);
            }

            addressEntry.ApartmentNum = address.ApartmentNum;
            addressEntry.BlockNum = address.BlockNum;
            addressEntry.CountryId = address.CountryId;
            addressEntry.DistrictId = address.DistrictId;
            addressEntry.EntranceNum = address.EntranceNum;
            addressEntry.FloorNum = address.FloorNum;
            addressEntry.MunicipalityId = address.MunicipalityId;
            addressEntry.PopulatedAreaId = address.PopulatedAreaId;
            addressEntry.PostCode = address.PostalCode;
            addressEntry.Region = address.Region;
            addressEntry.Street = address.Street;
            addressEntry.StreetNum = address.StreetNum;

            if (!deferSave)
            {
                Db.SaveChanges();
            }

            return addressEntry;
        }
    }
}
