namespace PostIt.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}