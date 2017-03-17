using Domain.User;
using System;
using System.Linq;

namespace EF.Repositories
{
    class UserRepository : RepositoryBase<User, Guid>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public User Load(string userName)
        {
            return _context.Users.FirstOrDefault(e => e.Name == userName);
        }
    }
}
