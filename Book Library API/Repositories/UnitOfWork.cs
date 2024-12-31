using Bill_system_API.IRepositories;
using Bill_system_API.Models;

namespace Bill_system_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext context;
        public IGenericRepository<Book> Books { get; private set; }

        public IBorrowedBookRepository BorrowedBooks { get; private set; }

        public UnitOfWork(LibraryDbContext _contect)
        {
            context= _contect;
            Books=new GenericRepository<Book>(context);
            BorrowedBooks= new BorrowedBookRepository(context);
        }
        public int Complete()
        {
            return context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
