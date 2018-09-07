namespace Baicao.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Consumers",
                c => new
                    {
                        Openid = c.String(nullable: false, maxLength: 128),
                        Mobilephone = c.String(maxLength: 20),
                        Regdate = c.DateTime(nullable: false),
                        Couponcode = c.String(maxLength: 32),
                        Updatetime = c.DateTime(nullable: false),
                        Userip = c.String(maxLength: 32),
                        Dadacode = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Openid);
            
            CreateTable(
                "dbo.Invitions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConsumerOpenid = c.String(maxLength: 128),
                        InvOpenid = c.String(maxLength: 128),
                        Invdate = c.DateTime(nullable: false),
                        MatchType = c.Int(nullable: false),
                        Iftmall = c.Boolean(nullable: false),
                        Tmalldate = c.DateTime(nullable: false),
                        Updatetime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WxUserInfoes", t => t.ConsumerOpenid)
                .ForeignKey("dbo.WxUserInfoes", t => t.InvOpenid)
                .Index(t => t.ConsumerOpenid)
                .Index(t => t.InvOpenid);
            
            CreateTable(
                "dbo.WxUserInfoes",
                c => new
                    {
                        Openid = c.String(nullable: false, maxLength: 128),
                        NickName = c.String(),
                        HeadImgUrl = c.String(),
                        CreateOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Openid);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SmsCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreateOn = c.DateTime(nullable: false),
                        UserIp = c.String(maxLength: 32),
                        Mobiphone = c.String(maxLength: 20),
                        CodeHash = c.String(maxLength: 32),
                        Salt = c.String(maxLength: 32),
                        PlainCode = c.String(maxLength: 16),
                        State = c.Int(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Invitions", "InvOpenid", "dbo.WxUserInfoes");
            DropForeignKey("dbo.Invitions", "ConsumerOpenid", "dbo.WxUserInfoes");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Invitions", new[] { "InvOpenid" });
            DropIndex("dbo.Invitions", new[] { "ConsumerOpenid" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SmsCodes");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.WxUserInfoes");
            DropTable("dbo.Invitions");
            DropTable("dbo.Consumers");
        }
    }
}
