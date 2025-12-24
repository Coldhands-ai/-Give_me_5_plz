using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PayrollSystem.Models
{
    public class Payroll
    {
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public string Period { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal WorkedHours { get; set; }
        public decimal Bonus { get; set; }
        public decimal Penalty { get; set; }
        public decimal NetSalary { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; }

        [NotMapped]
        public string EmployeeName => Employee != null ? $"{Employee.LastName} {Employee.FirstName}" : "";

        [NotMapped]
        public decimal TotalDeductions => PayrollDeductions?.Sum(pd => pd.Amount) ?? 0;
    }
}
