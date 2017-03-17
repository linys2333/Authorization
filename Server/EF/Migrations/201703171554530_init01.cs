namespace EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.p_User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Pwd = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.p_User");
        }
    }
}
