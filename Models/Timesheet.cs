using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class Timesheet
    {
        public int TimesheetID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime DateWorked { get; set; }
        public decimal Hours { get; set; }
        public virtual Employee Employee { get; set; }

        [NotMapped]
        public string EmployeeName => Employee != null ? $"{Employee.LastName} {Employee.FirstName}" : "";
    }
}
