using Microsoft.EntityFrameworkCore;
using PostIt.Data.Interfaces;
using PostIt.Domain.Entities;

namespace PostIt.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly PostItContext _context;

        public ClientRepository(PostItContext context)
        {
            _context = context;
        }

        public async Task<Client> GetClientByNameAndAddressAsync(string name, string address)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Name == name && c.Address == address);
        }

        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task AddClientAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }
    }
}