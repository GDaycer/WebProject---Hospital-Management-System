namespace WebProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeHeightWeightNullable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                {
                    appointmentId = c.Int(nullable: false, identity: true),
                    patientId = c.Int(nullable: false),
                    doctorId = c.Int(nullable: false),
                    startTime = c.DateTime(nullable: false),
                    endTime = c.DateTime(nullable: false),
                    status = c.String(),
                    ReasonForVisit = c.String(storeType: "ntext"),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(),
                })
                .PrimaryKey(t => t.appointmentId)
                .ForeignKey("dbo.Users", t => t.doctorId, cascadeDelete: false)
                .ForeignKey("dbo.Patients", t => t.patientId, cascadeDelete: false)
                .Index(t => t.patientId)
                .Index(t => t.doctorId);

            AddColumn("dbo.Patients", "Height", c => c.Int());
            AddColumn("dbo.Patients", "Weight", c => c.Int());
        }

        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "patientId", "dbo.Patients");
            DropForeignKey("dbo.Appointments", "doctorId", "dbo.Users");
            DropIndex("dbo.Appointments", new[] { "doctorId" });
            DropIndex("dbo.Appointments", new[] { "patientId" });
            DropColumn("dbo.Patients", "Weight");
            DropColumn("dbo.Patients", "Height");
            DropTable("dbo.Appointments");
        }
    }
}
