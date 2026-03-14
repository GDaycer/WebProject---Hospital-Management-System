namespace WebProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "PhoneNumber", c => c.String());
            DropColumn("dbo.Patients", "Phone_number");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Patients", "Phone_number", c => c.String());
            DropColumn("dbo.Patients", "PhoneNumber");
        }
    }
}
