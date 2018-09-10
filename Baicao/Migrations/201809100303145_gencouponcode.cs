namespace Baicao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gencouponcode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CouponCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 32),
                        CreateOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "dbo.DadaCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 10),
                        CreateOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            AddColumn("dbo.Consumers", "CodeId", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            DropIndex("dbo.DadaCodes", new[] { "Code" });
            DropIndex("dbo.CouponCodes", new[] { "Code" });
            DropColumn("dbo.Consumers", "CodeId");
            DropTable("dbo.DadaCodes");
            DropTable("dbo.CouponCodes");
        }
    }
}
