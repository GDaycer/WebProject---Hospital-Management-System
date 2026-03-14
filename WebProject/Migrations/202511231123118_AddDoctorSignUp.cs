namespace WebProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctorSignUp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorSignUps",
                c => new
                    {
                        DoctorSignUpId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Specialty = c.String(nullable: false),
                        LicenseNumber = c.String(nullable: false),
                        Bio = c.String(),
                        ShiftStart = c.Time(precision: 7),
                        ShiftEnd = c.Time(precision: 7),
                    })
                .PrimaryKey(t => t.DoctorSignUpId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DoctorSignUps");
        }
    }
}
