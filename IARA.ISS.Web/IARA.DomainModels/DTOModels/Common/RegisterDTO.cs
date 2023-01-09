using IARA.DomainModels.RequestModels;

namespace IARA.DomainModels.DTOModels.Common
{
    public class RegisterDTO<T>
    {
        public T Dto { get; set; }

        public PrintConfigurationParameters PrintConfiguration { get; set; }
    }
}
