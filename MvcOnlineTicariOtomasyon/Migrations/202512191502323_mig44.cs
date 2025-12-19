namespace MvcOnlineTicariOtomasyon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig44 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KargoDetays", "TakipKodu", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("dbo.KargoDetays", "Tarih", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KargoDetays", "Tarih", c => c.DateTime(nullable: false));
            AlterColumn("dbo.KargoDetays", "TakipKodu", c => c.String(maxLength: 10, unicode: false));
        }
    }
}
