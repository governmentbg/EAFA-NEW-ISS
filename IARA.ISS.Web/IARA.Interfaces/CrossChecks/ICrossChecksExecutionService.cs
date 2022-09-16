namespace IARA.Interfaces.CrossChecks
{
    public interface ICrossChecksExecutionService
    {
        int ExecuteCrossCheck(int crossCheckId);
        void ExecuteCrossChecks(string execFrequency);
    }
}
