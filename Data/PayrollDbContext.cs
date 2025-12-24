using System.Data.Entity;
using PayrollSystem.Models;

namespace PayrollSystem.Data
{
    public class PayrollDbContext : DbContext
    {
        public PayrollDbContext() : base("name=PayrollConnection")
        {
            Database.SetInitializer<PayrollDbContext>(null);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<PayrollDeduction> PayrollDeductions { get; set; }
        public DbSet<ProductionCalendar> ProductionCalendars { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Position>().ToTable("Positions");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Timesheet>().ToTable("Timesheet");
            modelBuilder.Entity<Payroll>().ToTable("Payroll");
            modelBuilder.Entity<Deduction>().ToTable("Deductions");
            modelBuilder.Entity<PayrollDeduction>().ToTable("PayrollDeductions");
            modelBuilder.Entity<ProductionCalendar>().ToTable("ProductionCalendar");
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<Employee>().HasKey(e => e.EmployeeID);
            modelBuilder.Entity<Position>().HasKey(p => p.PositionID);
            modelBuilder.Entity<Department>().HasKey(d => d.DepartmentID);
            modelBuilder.Entity<Timesheet>().HasKey(t => t.TimesheetID);
            modelBuilder.Entity<Payroll>().HasKey(p => p.PayrollID);
            modelBuilder.Entity<Deduction>().HasKey(d => d.DeductionID);
            modelBuilder.Entity<PayrollDeduction>().HasKey(pd => pd.PayrollDeductionID);
            modelBuilder.Entity<ProductionCalendar>().HasKey(pc => pc.CalendarID);
            modelBuilder.Entity<User>().HasKey(u => u.UserID);

            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionID);

            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentID);

            modelBuilder.Entity<Timesheet>()
                .HasRequired(t => t.Employee)
                .WithMany(e => e.Timesheets)
                .HasForeignKey(t => t.EmployeeID);

            modelBuilder.Entity<Payroll>()
                .HasRequired(p => p.Employee)
                .WithMany(e => e.Payrolls)
                .HasForeignKey(p => p.EmployeeID);

            modelBuilder.Entity<PayrollDeduction>()
                .HasRequired(pd => pd.Payroll)
                .WithMany(p => p.PayrollDeductions)
                .HasForeignKey(pd => pd.PayrollID);

            modelBuilder.Entity<PayrollDeduction>()
                .HasRequired(pd => pd.Deduction)
                .WithMany(d => d.PayrollDeductions)
                .HasForeignKey(pd => pd.DeductionID);

            modelBuilder.Entity<Employee>().Ignore(e => e.FullName);
            modelBuilder.Entity<Employee>().Ignore(e => e.PositionName);
            modelBuilder.Entity<Employee>().Ignore(e => e.DepartmentName);
            modelBuilder.Entity<Timesheet>().Ignore(t => t.EmployeeName);
            modelBuilder.Entity<Payroll>().Ignore(p => p.EmployeeName);
            modelBuilder.Entity<Payroll>().Ignore(p => p.TotalDeductions);
            modelBuilder.Entity<PayrollDeduction>().Ignore(pd => pd.DeductionName);
            modelBuilder.Entity<ProductionCalendar>().Ignore(pc => pc.MonthName);
            modelBuilder.Entity<ProductionCalendar>().Ignore(pc => pc.DisplayText);

            base.OnModelCreating(modelBuilder);
        }
    }
}
