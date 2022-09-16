using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NNomenclatureTables
    {
        public static List<NnomenclatureTable> NomenclatureTables
        {
            get
            {
                return new List<NnomenclatureTable>
                {
                    new NnomenclatureTable { CanDeleteRows=true, CanEditRows=true, CanInsertRows=true, Description="Видове адреси", IsActive=true, Name="NAddressTypes", SchemaName="Legals"  },
                    new NnomenclatureTable { CanDeleteRows=true, CanEditRows=true, CanInsertRows=true, Description="Области", IsActive=true, Name="NDistricts", SchemaName="Noms"},
                    new NnomenclatureTable { CanDeleteRows=true, CanEditRows=true, CanInsertRows=true, Description="Общини", IsActive=true, Name="NMunicipalities", SchemaName="Noms"},
                    new NnomenclatureTable { CanDeleteRows=true, CanEditRows=true, CanInsertRows=true, Description="Населени места", IsActive=true, Name="NPopulatedAreas", SchemaName="Noms"},
                    new NnomenclatureTable { CanDeleteRows=true, CanEditRows=true, CanInsertRows=true, Description="Териториални звена", IsActive=true, Name="NTerritoryUnits", SchemaName="Noms"},
                };
            }
        }
    }
}
