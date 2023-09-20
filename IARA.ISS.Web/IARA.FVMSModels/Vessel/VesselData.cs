using System;

namespace IARA.FVMSModels.Vessel
{
    public class VesselData
    {
        // Код на събитието
        public string EventCode { get; set; }

        // Дата на събитието
        public DateTime EventDate { get; set; }

        // Име на кораба
        public string Name { get; set; }

        // Външна маркировка
        public string ExternalMark { get; set; }

        // IRCS позивна
        public string IRCSCallSign { get; set; }

        // MMSI
        public string MMSI { get; set; }

        // UVI
        public string UVI { get; set; }

        // Флаг за наличие на AIS
        public bool HasAIS { get; set; }

        // Флаг за наличие на ERS
        public bool HasERS { get; set; }

        // Флаг за наличие на VMS
        public bool HasVMS { get; set; }

        // Код на пристанище
        public string PortCode { get; set; }

        // Дължина на кораба
        public decimal TotalLength { get; set; }

        // Ширина на кораба
        public decimal TotalWidth { get; set; }

        // Бруто тонаж
        public decimal GrossTonnage { get; set; }

        // Нето тонаж (може да няма)
        public decimal? NetTonnage { get; set; }

        // Дължина между перпендикулярите (може да няма)
        public decimal? LengthBetweenPerpendiculars { get; set; }

        // Мощност на главния двигател (kW)
        public decimal MainEnginePower { get; set; }

        // Мощност на спомагателния двигател (kW), ако има такъв
        public decimal? AuxiliaryEnginePower { get; set; }

        // Код на основния риболовен уред
        public string MainFishingGearCode { get; set; }

        // Код на допълнителен риболовен уред (ако има такъв)
        public string AdditionalFishingGearCode { get; set; }

        // Код на материала на корпуса
        public string HullMaterialCode { get; set; }
    }
}
