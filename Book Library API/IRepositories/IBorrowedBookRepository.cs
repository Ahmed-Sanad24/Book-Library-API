using Bill_system_API.Models;

namespace Bill_system_API.IRepositories
{
    public interface IBorrowedBookRepository : IGenericRepository<BorrowedBook>
    {
        IEnumerable<BorrowedBook> GetBorrowedBooksByUserId(string userId);
        IEnumerable<BorrowedBook> GetOverdueBooks();
    }
}
