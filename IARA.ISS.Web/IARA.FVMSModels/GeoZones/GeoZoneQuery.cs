using System;

namespace IARA.FVMSModels
{
    public class GeoZoneQuery
    {
        public Guid Identifier { get; set; }

        /// <summary>
        /// Тип на географската зона
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Целочислена стойност(секунди)
        /// </summary>
        public int Since { get; set; }
    }
}
