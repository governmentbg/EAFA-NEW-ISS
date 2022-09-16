namespace IARA.Common.TempFileUtils
{
    public interface IFilesSweeper
    {
        void AddFileForRemoval(TempFileStream fileStream);
        void Start();
    }
}