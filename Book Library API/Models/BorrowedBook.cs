using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bill_system_API.Models
{
    public class BorrowedBook
    {
        public int Id { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue()
        {
            return ReturnDate == null && (DateTime.Now - BorrowedDate).TotalSeconds > 14;
        }
    }
}
