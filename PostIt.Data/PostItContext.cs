using Microsoft.EntityFrameworkCore;
using PostIt.Domain.Entities;

namespace PostIt.Data
{
    public class PostItContext : DbContext
    {
        public PostItContext(DbContextOptions<PostItContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}