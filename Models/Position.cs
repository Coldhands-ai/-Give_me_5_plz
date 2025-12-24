using System.Collections.Generic;

namespace PayrollSystem.Models
{
    public class Position
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; }
        public decimal BaseSalary { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
