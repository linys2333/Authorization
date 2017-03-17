using System;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Interfaces;

namespace Domain.User
{
    [Table("p_User")]
    public class User : Entity<Guid>
    {
        public string Name { get; set; }
        public string Pwd { get; set; }
    }

    public interface IUserRepository : IRepository<User, Guid>
    {
        User Load(string userName);
    }
}
