namespace SeanProfile.Api.Model
{
    public class TodoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}