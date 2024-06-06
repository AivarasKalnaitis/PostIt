using PostIt.Domain.Entities;

namespace PostIt.Data.Interfaces
{
    public interface ILogRepository
    {
        Task AddLog(Log log);
    }
}