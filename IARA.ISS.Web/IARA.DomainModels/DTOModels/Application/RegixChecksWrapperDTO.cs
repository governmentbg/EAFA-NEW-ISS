namespace IARA.DomainModels.DTOModels.Application
{
    public class RegixChecksWrapperDTO<T> where T : class
    {
        public T DialogDataModel { get; set; }

        public T RegiXDataModel { get; set; }
    }
}
