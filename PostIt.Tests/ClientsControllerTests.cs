namespace PostIt.Tests
{
    using Xunit;
    using Moq;
    using Microsoft.AspNetCore.Mvc;
    using PostIt.API.Controllers;
    using PostIt.Data.Interfaces;
    using PostIt.Services;
    using Microsoft.Extensions.Configuration;
    using PostIt.Domain.Entities;

    public class ClientsControllerTests
    {
        private ClientsController CreateController(Mock<IClientRepository> mockClientRepository, Mock<ILogRepository> mockLogRepository, Mock<IConfiguration> mockConfiguration, Mock<IFileService> mockFileService)
        {
            return new ClientsController(mockClientRepository.Object, mockLogRepository.Object, mockConfiguration.Object, mockFileService.Object);
        }

        [Fact]
        public async Task ImportClients_ValidJsonData_ReturnsOkResult()
        {
            var mockClientRepository = new Mock<IClientRepository>();
            var mockLogRepository = new Mock<ILogRepository>();
            var mockConfiguration = new Mock<IConfiguration>();

            var jsonData = "[{\"name\": \"John Doe\", \"address\": \"123 Main St\"}, {\"name\": \"Jane Smith\", \"address\": \"456 Elm St\"}]";
            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(fs => fs.ReadJsonData(It.IsAny<string>())).ReturnsAsync(jsonData);

            var controller = CreateController(mockClientRepository, mockLogRepository, mockConfiguration, mockFileService);

            var result = await controller.ImportClients();

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task ImportClients_InvalidJsonData_ReturnsNotFoundResult()
        {
            var mockClientRepository = new Mock<IClientRepository>();
            var mockLogRepository = new Mock<ILogRepository>();
            var mockConfiguration = new Mock<IConfiguration>();

            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(fs => fs.ReadJsonData(It.IsAny<string>())).ThrowsAsync(new FileNotFoundException());

            var controller = CreateController(mockClientRepository, mockLogRepository, mockConfiguration, mockFileService);

            var result = await controller.ImportClients();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Error importing clients: Unable to find the specified file.", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdatePostCodes_SuccessfulResponse_ReturnsOkResult()
        {
            var mockClientRepository = new Mock<IClientRepository>();
            var mockLogRepository = new Mock<ILogRepository>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockFileService = new Mock<IFileService>();

            var clients = new List<Client>
            {
                new Client { Id = 1, Name = "Client1", Address = "123 Main St" },
                new Client { Id = 2, Name = "Client2", Address = "456 Elm St" }
            };
            mockClientRepository.Setup(repo => repo.GetClients()).ReturnsAsync(clients);
            mockConfiguration.Setup(cfg => cfg["PostitApiKey"]).Returns("dummy-api-key");
            mockFileService.Setup(fs => fs.ReadJsonData(It.IsAny<string>())).ReturnsAsync("{\"data\": [{\"post_code\": \"12345\"}]}");

            var controller = CreateController(mockClientRepository, mockLogRepository, mockConfiguration, mockFileService);

            var result = await controller.UpdatePostCodes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Post codes updated successfully", okResult.Value);
        }

        [Fact]
        public async Task UpdatePostCodes_UnsuccessfulResponse_ReturnsOkResult()
        {
            var mockClientRepository = new Mock<IClientRepository>();
            var mockLogRepository = new Mock<ILogRepository>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockFileService = new Mock<IFileService>();

            var clients = new List<Client>
            {
                new Client { Id = 1, Name = "Client1", Address = "123 Main St" },
                new Client { Id = 2, Name = "Client2", Address = "456 Elm St" }
            };
            mockClientRepository.Setup(repo => repo.GetClients()).ReturnsAsync(clients);
            mockConfiguration.Setup(cfg => cfg["PostitApiKey"]).Returns("dummy-api-key");
            mockFileService.Setup(fs => fs.ReadJsonData(It.IsAny<string>())).ReturnsAsync("{\"data\": [{\"post_code\": \"12345\"}]}");

            var controller = CreateController(mockClientRepository, mockLogRepository, mockConfiguration, mockFileService);

            var result = await controller.UpdatePostCodes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Post codes updated successfully", okResult.Value);
        }
    }
}