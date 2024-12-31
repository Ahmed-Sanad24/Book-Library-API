public class BorrowedBookDTO
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserId { get; set; }
    public DateTime BorrowedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
}
