using PostIt.Domain.Entities;

namespace PostIt.Data.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> GetClientByNameAndAddressAsync(string name, string address);
        Task<IEnumerable<Client>> GetClientsAsync();
        Task AddClientAsync(Client client);
    }
}