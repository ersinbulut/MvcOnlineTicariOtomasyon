namespace MvcOnlineTicariOtomasyon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig46 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mesajlars", "Okundu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Mesajlars", "Silindi", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mesajlars", "Silindi");
            DropColumn("dbo.Mesajlars", "Okundu");
        }
    }
}
