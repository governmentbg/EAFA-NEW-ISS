using IARA.Mobile.Domain.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Pub.Domain.Entities.Nomenclatures
{
    public class NTerritorialUnit : INomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
