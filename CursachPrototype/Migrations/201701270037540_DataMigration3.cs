namespace CursachPrototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserNickName", c => c.String());
            DropColumn("dbo.AspNetUsers", "UserDbSuffix");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserDbSuffix", c => c.String());
            DropColumn("dbo.AspNetUsers", "UserNickName");
        }
    }
}
