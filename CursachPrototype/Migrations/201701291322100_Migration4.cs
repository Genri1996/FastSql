namespace CursachPrototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "UserNickName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserNickName", c => c.String());
        }
    }
}
