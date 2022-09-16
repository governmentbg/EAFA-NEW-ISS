using System;

namespace IARA.FVMSModels.Stuctures
{
    public struct Position
    {
        /// <summary>
        /// Географска дължина в градуси WGS84
        /// </summary>
        public double Longiture { get; set; }

        /// <summary>
        /// Географска ширина в градуси WGS84
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Дата на позицията
        /// </summary>
        public DateTime CoordinatesTime { get; set; }

        /// <summary>
        /// Скорост
        /// </summary>
        public uint Speed { get; set; }

        /// <summary>
        /// Курс
        /// </summary>
        public int Course { get; set; }
    }
}
