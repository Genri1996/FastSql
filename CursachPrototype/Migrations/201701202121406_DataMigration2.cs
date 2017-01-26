namespace CursachPrototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataBaseInfoes", "DateOfCreating", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataBaseInfoes", "DateOfCreating");
        }
    }
}
