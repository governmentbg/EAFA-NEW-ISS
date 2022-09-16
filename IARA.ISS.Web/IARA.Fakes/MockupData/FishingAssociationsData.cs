using System;
using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class FishingAssociationsData
    {
        public static List<FishingAssociation> FishingAssociations
        {
            get
            {
                return new List<FishingAssociation>
                {
                    new FishingAssociation
                    {
                        Id = 1,
                        AssociationLegalId = LegalsData.Legals[0].Id,
                        TerritoryUnitId = TerritoryData.TerritoryUnits[0].Id
                    },
                    new FishingAssociation
                    {
                        Id = 2,
                        AssociationLegalId = LegalsData.Legals[1].Id,
                        TerritoryUnitId = TerritoryData.TerritoryUnits[1].Id,
                        IsActive = false
                    },
                    new FishingAssociation
                    {
                        Id = 3,
                        AssociationLegalId = LegalsData.Legals[2].Id,
                        TerritoryUnitId = TerritoryData.TerritoryUnits[2].Id,
                        IsCanceled = true,
                        CancellationDate = new DateTime(2021, 4, 23),
                        CancellationReason = "де да знам",
                        IsActive = true
                    }
                };
            }
        }
    }
}
