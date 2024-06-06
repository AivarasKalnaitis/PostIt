using PostIt.Data.Interfaces;
using PostIt.Domain.Entities;

namespace PostIt.Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly PostItContext _context;

        public LogRepository(PostItContext context)
        {
            _context = context;
        }

        public async Task AddLog(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}