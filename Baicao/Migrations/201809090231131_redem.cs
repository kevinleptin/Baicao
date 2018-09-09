namespace Baicao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class redem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dealers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Source = c.String(maxLength: 50),
                        Name = c.String(maxLength: 50),
                        RedemCode = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Redems",
                c => new
                    {
                        CouponCode = c.String(nullable: false, maxLength: 16),
                        RedemDate = c.DateTime(nullable: false),
                        RedemSource = c.String(maxLength: 32),
                        RedemPerson = c.String(maxLength: 32),
                        RedemCode = c.String(maxLength: 32),
                        RedemProduct = c.String(maxLength: 32),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CouponCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Redems");
            DropTable("dbo.Dealers");
        }
    }
}
