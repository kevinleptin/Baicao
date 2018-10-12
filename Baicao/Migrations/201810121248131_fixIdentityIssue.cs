namespace Baicao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixIdentityIssue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CouponCodes", "BakId", c => c.Int(nullable: false));
            AddColumn("dbo.DadaCodes", "BakId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DadaCodes", "BakId");
            DropColumn("dbo.CouponCodes", "BakId");
        }
    }
}
