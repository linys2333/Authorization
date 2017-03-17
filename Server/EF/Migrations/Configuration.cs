using System.Data.Entity.Migrations;

namespace EF.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DbContext context)
        {

        }
    }
}
