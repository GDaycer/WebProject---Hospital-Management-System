namespace WebProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        DoctorId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Specialty = c.String(),
                        LicenseNumber = c.String(),
                        Bio = c.String(storeType: "ntext"),
                        ShiftStart = c.Time(precision: 7),
                        ShiftEnd = c.Time(precision: 7),
                    })
                .PrimaryKey(t => t.DoctorId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Doctors", "UserId", "dbo.Users");
            DropIndex("dbo.Doctors", new[] { "UserId" });
            DropTable("dbo.Doctors");
        }
    }
}
