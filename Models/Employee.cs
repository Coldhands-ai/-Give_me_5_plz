using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public int PositionID { get; set; }
        public int DepartmentID { get; set; }
        public decimal BaseSalary { get; set; }
        public virtual Position Position { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Timesheet> Timesheets { get; set; }
        public virtual ICollection<Payroll> Payrolls { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        [NotMapped]
        public string PositionName => Position?.PositionName ?? "";

        [NotMapped]
        public string DepartmentName => Department?.DepartmentName ?? "";
    }
}
