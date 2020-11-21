namespace TaskManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        id_com = c.Int(nullable: false, identity: true),
                        id_us = c.Int(nullable: false),
                        id_tsk = c.Int(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.id_com)
                .ForeignKey("dbo.Tasks", t => t.id_tsk, cascadeDelete: true)
                .Index(t => t.id_tsk);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        id_tsk = c.Int(nullable: false, identity: true),
                        id_pr = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Status = c.String(),
                        Date_St = c.DateTime(nullable: false),
                        Date_End = c.DateTime(nullable: false),
                        id_us = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id_tsk)
                .ForeignKey("dbo.Projects", t => t.id_pr, cascadeDelete: true)
                .Index(t => t.id_pr);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        id_pr = c.Int(nullable: false, identity: true),
                        id_team = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Date_St = c.DateTime(nullable: false),
                        Date_End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id_pr)
                .ForeignKey("dbo.Teams", t => t.id_team, cascadeDelete: true)
                .Index(t => t.id_team);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        id_team = c.Int(nullable: false, identity: true),
                        id_org = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id_team);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "id_team", "dbo.Teams");
            DropForeignKey("dbo.Tasks", "id_pr", "dbo.Projects");
            DropForeignKey("dbo.Comments", "id_tsk", "dbo.Tasks");
            DropIndex("dbo.Projects", new[] { "id_team" });
            DropIndex("dbo.Tasks", new[] { "id_pr" });
            DropIndex("dbo.Comments", new[] { "id_tsk" });
            DropTable("dbo.Teams");
            DropTable("dbo.Projects");
            DropTable("dbo.Tasks");
            DropTable("dbo.Comments");
        }
    }
}
