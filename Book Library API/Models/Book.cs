namespace Bill_system_API.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int? PublishedYear { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
