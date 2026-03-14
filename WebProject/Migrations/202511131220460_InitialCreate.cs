namespace WebProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        BirthDate = c.DateTime(),
                        Gender = c.String(),
                        Address = c.String(),
                        Phone_number = c.String(),
                        EmergencyContactName = c.String(),
                        EmergencyContactNumber = c.String(),
                        BloodType = c.String(),
                    })
                .PrimaryKey(t => t.PatientId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Password = c.String(),
                        Role = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "UserId", "dbo.Users");
            DropIndex("dbo.Patients", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Patients");
        }
    }
}
