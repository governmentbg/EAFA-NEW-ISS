using System;

namespace IARA.FVMSModels.FA
{
    public class FATripMessage
    {
        /// <summary>
        /// Уникален идентификатор на съобщението
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// 9, 5, 3, 1
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// NOTIFICATION, DECLARATION
        /// </summary>
        public string FaReportType { get; set; }

        /// <summary>
        /// DEPARTURE, ARRIVAL, FISHING_OPERATION, etc...
        /// </summary>
        public string FaType { get; set; }

        /// <summary>
        /// Дата и час на постъпване в ИИС
        /// </summary>
        public DateTime ReceiveDate { get; set; }
    }
}
