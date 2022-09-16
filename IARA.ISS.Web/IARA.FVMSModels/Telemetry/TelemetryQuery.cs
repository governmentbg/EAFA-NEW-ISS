using System;

namespace IARA.FVMSModels
{
    public class TelemetryQuery
    {
        /// <summary>
        /// Уникален идентификатор на направената заявка. Може да се генерира UUID номер
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// CFR
        /// </summary>
        public string VesselIdentifier { get; set; }

        /// <summary>
        /// Указва с каква актуалност да бъдат данните. 
        /// При стойност 0 – последна стойност; 
        /// При стойност 600 – СНРК отговаря само ако има в рамките на 600 сeкунди изменение в стойностите на параметрите.
        /// </summary>
        public int Since { get; set; }
    }
}
