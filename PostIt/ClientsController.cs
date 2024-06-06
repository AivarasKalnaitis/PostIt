using Microsoft.AspNetCore.Mvc;
using PostIt.Domain.Entities;
using Newtonsoft.Json;
using PostIt.Data.Interfaces;
using PostIt.Services;

namespace PostIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        public ClientsController(IClientRepository clientRepository, ILogRepository logRepository, IConfiguration configuration, IFileService fileService)
        {
            _clientRepository = clientRepository;
            _logRepository = logRepository;
            _configuration = configuration;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _clientRepository.GetClients();
            return Ok(clients);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportClients()
        {
            try
            {
                var jsonData = await _fileService.ReadJsonData("Resources/klientai.json");

                var clients = JsonConvert.DeserializeObject<List<Client>>(jsonData);
                if (clients == null)
                {
                    return BadRequest("Error deserializing client data.");
                }

                foreach (var client in clients)
                {
                    var existingClient = await _clientRepository.GetClientByNameAndAddress(client.Name, client.Address);
                    if (existingClient == null)
                    {
                        await _clientRepository.AddClient(client);
                        await _logRepository.AddLog(new Log { Action = $"Imported client {client.Name}" });
                    }
                }

                return Ok("Clients imported successfully");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Error importing clients: Unable to find the specified file.");
            }
            catch (Exception ex)
            {
                await _logRepository.AddLog(new Log { Action = $"Error importing clients: {ex.Message}" });
                return StatusCode(500, "An error occurred while importing clients.");
            }
        }


        [HttpPost("update_post_codes")]
        public async Task<IActionResult> UpdatePostCodes()
        {
            var clients = await _clientRepository.GetClients();
            var postitApiKey = _configuration["PostitApiKey"];

            using (var httpClient = new HttpClient())
            {
                foreach (var client in clients)
                {
                    var address = client.Address.Replace(" ", "+");
                    var apiUrl = $"https://api.postit.lt/?term={address}&key={postitApiKey}";

                    var response = await httpClient.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseData);
                        if (jsonResponse?.data != null && jsonResponse.data.HasValues)
                        {
                            var postCode = jsonResponse.data[0]?.post_code;
                            if (postCode != null)
                            {
                                client.PostCode = postCode;
                                client.UpdatedAt = DateTime.Now;
                                await _logRepository.AddLog(new Log { Action = $"Updated post code for {client.Name}" });
                            }
                        }
                    }
                }
            }

            return Ok("Post codes updated successfully");
        }
    }
}