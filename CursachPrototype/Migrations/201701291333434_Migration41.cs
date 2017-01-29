namespace CursachPrototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration41 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataBaseInfoes", "ConnectionString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataBaseInfoes", "ConnectionString");
        }
    }
}
