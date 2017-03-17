using System;
using Common.Interfaces;
using AnyExtend;

namespace EF
{
    class RepositoryBase<T, K> : IRepository<T, K>
        where T : class, IEntity<K>
    {
        protected readonly DbContext _context;

        public RepositoryBase(DbContext context)
        {
            _context = context;
        }

        public T Create()
        {
            return _context.Set<T>().Create();
        }

        public void Delete(K id)
        {
            Delete(Get(id));
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public T Get(K id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Insert(T entity)
        {
            entity.Id = GenerateNewId();
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Save(T entity)
        {
            if (entity.Id == null || entity.Id.Equals(default(K)))
            {
                entity.Id = GenerateNewId();
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public void Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        protected K GenerateNewId()
        {
            if (typeof(K) == typeof(Guid))
            {
                return TypeExt.ConvertType<K>(Util.SeqGuid());
            }

            if (typeof(K) == typeof(int) || typeof(K) == typeof(long))
            {
                return TypeExt.ConvertType<K>(0);
            }

            throw new NotSupportedException("不支持的主键类型");
        }
    }
}
