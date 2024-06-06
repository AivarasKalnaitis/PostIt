namespace PostIt.Services
{
    public class FileService : IFileService
    {
        private string _jsonData;
        private DateTime _lastModified;

        public async Task<string> ReadJsonData(string relativePath)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var fullPath = Path.Combine(baseDirectory, relativePath);

            if (!IsFileModified(fullPath))
                return _jsonData;

            _lastModified = File.GetLastWriteTimeUtc(fullPath);
            _jsonData = await File.ReadAllTextAsync(fullPath);
            return _jsonData;
        }

        private bool IsFileModified(string filePath)
        {
            var lastModified = File.GetLastWriteTimeUtc(filePath);
            return lastModified > _lastModified;
        }
    }
}