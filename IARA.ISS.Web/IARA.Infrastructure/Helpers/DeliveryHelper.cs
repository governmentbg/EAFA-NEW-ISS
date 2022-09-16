using System;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class DeliveryHelper
    {
        public static ApplicationDelivery AddDeliveryData(this IARADbContext db, ApplicationBaseDeliveryDTO deliveryData)
        {
            ApplicationDelivery entry = new ApplicationDelivery();

            UpdateCommonDeliveryFields(db, entry, deliveryData);

            return entry;
        }

        public static ApplicationDelivery AddDeliveryData(this IARADbContext db, Application dbApplication, IDeliverableApplication application)
        {
            if (application.DeliveryData != null)
            {
                dbApplication.Delivery = db.AddDeliveryData(application.DeliveryData);
                return dbApplication.Delivery;
            }

            return null;
        }

        public static ApplicationDelivery EditDeliveryData(this IARADbContext db, ApplicationBaseDeliveryDTO deliveryData, int oldDeliveryId)
        {
            ApplicationDelivery entry = (from delivery in db.ApplicationDeliveries
                                         where delivery.Id == oldDeliveryId
                                         select delivery).Single();

            UpdateCommonDeliveryFields(db, entry, deliveryData);

            return entry;
        }

        public static ApplicationDelivery EditDeliveryData(this IARADbContext db, ApplicationDeliveryDTO deliveryData, int oldDeliveryId)
        {
            ApplicationDelivery entry = (from delivery in db.ApplicationDeliveries
                                         where delivery.Id == oldDeliveryId
                                         select delivery).Single();

            UpdateCommonDeliveryFields(db, entry, deliveryData);

            entry.SentDate = deliveryData.SentDate;
            entry.ReferenceNum = deliveryData.ReferenceNumber;
            entry.IsDelivered = deliveryData.IsDelivered;
            entry.DeliveryDate = deliveryData.DeliveryDate;

            return entry;
        }

        public static ApplicationDelivery EditDeliveryData(this IARADbContext db, Application dbApplication, IDeliverableApplication application)
        {
            if (application.DeliveryData != null)
            {
                if (dbApplication.DeliveryId.HasValue)
                {
                    dbApplication.Delivery = db.EditDeliveryData(application.DeliveryData, dbApplication.DeliveryId.Value);
                }
                else
                {
                    dbApplication.Delivery = db.AddDeliveryData(application.DeliveryData);
                }

                return dbApplication.Delivery;
            }

            dbApplication.DeliveryId = null;
            return null;
        }

        private static void UpdateCommonDeliveryFields(IARADbContext db, ApplicationDelivery entry, ApplicationBaseDeliveryDTO deliveryData)
        {
            entry.DeliveryTypeId = deliveryData.DeliveryTypeId;

            DeliveryTypesEnum deliveryTypeCode = (from deliveryType in db.NdeliveryTypes
                                                  where deliveryType.Id == deliveryData.DeliveryTypeId
                                                  select Enum.Parse<DeliveryTypesEnum>(deliveryType.Code)).Single();

            switch (deliveryTypeCode)
            {
                case DeliveryTypesEnum.ByMail:
                    {
                        Address deliveryAddress = db.AddOrEditAddress(deliveryData.DeliveryAddress, true, entry.AddressId);
                        entry.Address = deliveryAddress;
                    }
                    break;
                case DeliveryTypesEnum.CopyOnEmail:
                    {
                        // TODO email ???
                    }
                    break;
                case DeliveryTypesEnum.Personal:
                    {
                        entry.TerritoryUnitId = deliveryData.DeliveryTeritorryUnitId;
                    }
                    break;
            }
        }
    }
}
