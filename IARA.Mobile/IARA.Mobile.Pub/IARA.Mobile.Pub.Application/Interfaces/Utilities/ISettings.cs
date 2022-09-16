using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Pub.Application.Interfaces.Utilities
{
    public interface ISettings
    {
        bool SuccessfulLogin { get; set; }

        /// <summary>
        /// The language that is currently chosen for translating the resources
        /// </summary>
        ResourceLanguageEnum CurrentResourceLanguage { get; set; }

        void Clear();
    }
}
