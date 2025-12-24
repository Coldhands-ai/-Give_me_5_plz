using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class ProductionCalendar
    {
        public int CalendarID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal WorkingHours { get; set; }

        [NotMapped]
        public string MonthName
        {
            get
            {
                string[] months = { "", "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
                                   "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
                return Month >= 1 && Month <= 12 ? months[Month] : "";
            }
        }

        [NotMapped]
        public string DisplayText => $"{MonthName} {Year}";
    }
}
