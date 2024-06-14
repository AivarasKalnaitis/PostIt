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
            var clients = await _clientRepository.GetClientsAsync();
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
                    var existingClient = await _clientRepository.GetClientByNameAndAddressAsync(client.Name, client.Address);
                    if (existingClient == null)
                    {
                        await _clientRepository.AddClientAsync(client);
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
            try
            {
                var clients = await _clientRepository.GetClientsAsync();
                var postitApiKey = _configuration["AppSettings:PostitApiKey"];

                if (string.IsNullOrEmpty(postitApiKey))
                {
                    return BadRequest("PostApiKey can not be null, check your configuration");
                }

                var updateResults = new List<(bool Success, string Message)>();

                foreach (var client in clients)
                {
                    var result = await UpdateClientPostCodeAsync(client, postitApiKey);
                    updateResults.Add(result);
                }

                var successes = updateResults.Where(r => r.Success).Select(r => r.Message).ToList();
                var errors = updateResults.Where(r => !r.Success).Select(r => r.Message).ToList();

                if (errors.Any())
                {
                    return BadRequest(new { message = "Some post codes could not be updated", errors });
                }

                return Ok(new { message = "Post codes updated successfully", successes });
            }
            catch (Exception ex)
            {
                await _logRepository.AddLog(new Log { Action = "Failed to update post codes", Exception = ex.ToString() });
                return StatusCode(500, "An unexpected error occurred while updating post codes.");
            }
        }

        private async Task<(bool Success, string Message)> UpdateClientPostCodeAsync(Client client, string postitApiKey)
        {
            var address = client.Address.Replace(" ", "+");
            var apiUrl = $"https://api.postit.lt/?term={address}&key={postitApiKey}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseData);

                        if (jsonResponse?.success == false)
                        {
                            var errorMessage = jsonResponse.message;
                            await _logRepository.AddLog(new Log { Action = $"Failed to update post code for {client.Name}. Error: {errorMessage}" });
                            return (Success: false, Message: $"Failed to update post code for {client.Name}. Error: {errorMessage}");
                        }

                        if (jsonResponse?.data != null && jsonResponse.data.HasValues)
                        {
                            var postCode = jsonResponse.data[0]?.post_code;
                            if (postCode != null)
                            {
                                client.PostCode = postCode;
                                client.UpdatedAt = DateTime.Now;
                                await _logRepository.AddLog(new Log { Action = $"Updated post code for {client.Name}" });
                                return (Success: true, Message: $"Updated post code for {client.Name}");
                            }
                        }

                        return (Success: true, Message: $"No data found for {client.Name}");
                    }
                    else
                    {
                        var errorMessage = $"HTTP Error: {response.StatusCode}";
                        await _logRepository.AddLog(new Log { Action = $"Failed to update post code for {client.Name}. Error: {errorMessage}" });
                        return (Success: false, Message: $"Failed to update post code for {client.Name}. Error: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                await _logRepository.AddLog(new Log { Action = $"Failed to update post code for {client.Name}. Exception: {ex.Message}" });
                return (Success: false, Message: $"Failed to update post code for {client.Name}. Exception: {ex.Message}");
            }
        }
    }
}