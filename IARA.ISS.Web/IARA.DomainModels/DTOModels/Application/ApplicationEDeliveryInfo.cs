using TL.EDelivery;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationEDeliveryInfo
    {
        public string Subject { get; set; }

        public byte[] DocBytes { get; set; }

        public string DocNameWithExtension { get; set; }

        public string DocRegNumber { get; set; }

        public eProfileType ReceiverType { get; set; }

        public string ReceiverUniqueIdentifier { get; set; }

        public string ReceiverPhone { get; set; }

        public string ReceiverEmail { get; set; }

        public string ServiceOID { get; set; }

        public string OperatorEGN { get; set; }
    }
}
