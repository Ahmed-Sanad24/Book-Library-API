using Bill_system_API.IRepositories;
using Bill_system_API.Models;

namespace Bill_system_API.Repositories
{
    public class BorrowedBookRepository : GenericRepository<BorrowedBook>, IBorrowedBookRepository
    {
        public BorrowedBookRepository(LibraryDbContext context) : base(context)
        {
        }

        public IEnumerable<BorrowedBook> GetBorrowedBooksByUserId(string userId)
        {
            return Find(b => b.UserId == userId);
        }
        public IEnumerable<BorrowedBook> GetOverdueBooks()
        {
            return Find(b => b.IsOverdue());
        }
    }
}
