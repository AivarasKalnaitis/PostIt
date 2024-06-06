using PostIt.Domain.Entities;

namespace PostIt.Data.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> GetClientByNameAndAddress(string name, string address);
        Task<IEnumerable<Client>> GetClients();
        Task AddClient(Client client);
    }
}