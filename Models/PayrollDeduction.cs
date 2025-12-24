using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class PayrollDeduction
    {
        public int PayrollDeductionID { get; set; }
        public int PayrollID { get; set; }
        public int DeductionID { get; set; }
        public decimal Amount { get; set; }
        public virtual Payroll Payroll { get; set; }
        public virtual Deduction Deduction { get; set; }

        [NotMapped]
        public string DeductionName => Deduction?.DeductionName ?? "";
    }
}
