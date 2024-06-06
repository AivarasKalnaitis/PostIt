namespace PostIt.Services
{
    public interface IFileService
    {
        Task<string> ReadJsonData(string filePath);
    }
}