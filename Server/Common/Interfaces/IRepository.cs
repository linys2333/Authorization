namespace Common.Interfaces
{
    public interface IRepository
    {
    }

    public interface IRepository<T, K> where T : class, IEntity<K>
    {
        T Create();

        T Get(K id);

        void Update(T entity);

        void Delete(T entity);

        void Delete(K id);

        void Insert(T entity);

        void Save(T entity);
    }
}