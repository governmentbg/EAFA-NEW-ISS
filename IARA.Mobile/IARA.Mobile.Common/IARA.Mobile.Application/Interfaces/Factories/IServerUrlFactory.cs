namespace IARA.Mobile.Application.Interfaces.Factories
{
    public interface IServerUrlFactory
    {
        string GetUrl(string environment);

        string GetExtension(string environment);

        string GetEnvironmentUrl(string environment, string name);
    }
}
