using System.Collections.Generic;

namespace PayrollSystem.Models
{
    public class Deduction
    {
        public int DeductionID { get; set; }
        public string DeductionName { get; set; }
        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; }
    }
}
