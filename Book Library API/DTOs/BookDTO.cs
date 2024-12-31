namespace Bill_system_API.DTOs
{
    public class CreateBookDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishedYear { get; set; }
    }

    public class UpdateBookDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int? PublishedYear { get; set; }
        public bool IsAvailable { get; set; }
    }
}
