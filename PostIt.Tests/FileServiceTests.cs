namespace PostIt.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;
    using Moq;
    using PostIt.Services;

    public class FileServiceTests
    {
        [Fact]
        public async Task ReadJsonData_FileExists_ReturnsJsonData()
        {
            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(fs => fs.ReadJsonData(It.IsAny<string>())).ReturnsAsync("{\"clients\": []}");

            var fileService = mockFileService.Object;

            var jsonData = await fileService.ReadJsonData("test.json");

            Assert.NotNull(jsonData);
            Assert.Equal("{\"clients\": []}", jsonData);
        }

        [Fact]
        public async Task ReadJsonData_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            var fileService = new FileService();

            await Assert.ThrowsAsync<FileNotFoundException>(() => fileService.ReadJsonData("nonexistent.json"));
        }
    }

}