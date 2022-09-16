using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TL.JasperReports.Integration.Models;

namespace TL.JasperReports.Integration
{
    public interface IJasperInputControlsClient : IDisposable
    {
        Task<InputControlsType> ListInputControlStructure(string pathToReport);
        Task<InputControlStateType> ListInputControlValues(string pathToReport);
        Task<InputControlStateType> SettingInputControlValues(string pathToReport, Dictionary<string, List<string>> inputControls);
    }
}
