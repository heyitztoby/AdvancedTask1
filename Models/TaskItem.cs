namespace TaskManagement.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string? TaskDescription { get; set; }
        public string? TaskPriority { get; set; }
        public string? TaskStatus { get; set; }
        public string? CustomerNo { get; set; }
    }
}