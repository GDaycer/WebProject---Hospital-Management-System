namespace WebProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctortables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedicalRecords",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        AppointmentId = c.Int(nullable: false),
                        DoctorId = c.Int(nullable: false),
                        Diagnosis = c.String(),
                        Notes = c.String(storeType: "ntext"),
                        Prescriptions = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: false)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: false)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: false)
                .Index(t => t.PatientId)
                .Index(t => t.AppointmentId)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MedicalRecords", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.MedicalRecords", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.MedicalRecords", "AppointmentId", "dbo.Appointments");
            DropIndex("dbo.MedicalRecords", new[] { "DoctorId" });
            DropIndex("dbo.MedicalRecords", new[] { "AppointmentId" });
            DropIndex("dbo.MedicalRecords", new[] { "PatientId" });
            DropTable("dbo.MedicalRecords");
        }
    }
}
