using Bill_system_API.IRepositories;
using Bill_system_API.Models;
using Bill_system_API.Repositories;

namespace Bill_system_API.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Book> Books { get; }
        IBorrowedBookRepository BorrowedBooks { get; }

        int Complete();
    }
}
