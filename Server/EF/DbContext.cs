using System.Data.Entity;
using Domain.User;
using System;

namespace EF
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext() : base("name=db") { }

        public DbSet<User> Users { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Console.Write("DbContext Dispose");
        }
    }
}