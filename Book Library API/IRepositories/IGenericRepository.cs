namespace Bill_system_API.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T getById(int id);
        void add(T entity);
        void delete(T entity);
        void update(T entity);
        IEnumerable<T> Find(Func<T, bool> predicate); // Add this method
        void save();
    }
}
