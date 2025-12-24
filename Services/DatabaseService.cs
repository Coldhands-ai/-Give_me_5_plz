using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PayrollSystem.Data;
using PayrollSystem.Models;

namespace PayrollSystem.Services
{
    public class DatabaseService
    {
        #region Employees

        public List<Employee> GetAllEmployees()
        {
            using (var db = new PayrollDbContext())
            {
                return db.Employees
                    .Include(e => e.Position)
                    .Include(e => e.Department)
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToList();
            }
        }

        public void AddEmployee(Employee employee)
        {
            using (var db = new PayrollDbContext())
            {
                db.Employees.Add(employee);
                db.SaveChanges();
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            using (var db = new PayrollDbContext())
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void DeleteEmployee(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var employee = db.Employees.Find(id);
                if (employee != null)
                {
                    db.Employees.Remove(employee);
                    db.SaveChanges();
                }
            }
        }

        public decimal GetEmployeeSalary(int employeeId)
        {
            using (var db = new PayrollDbContext())
            {
                return db.Employees.Find(employeeId)?.BaseSalary ?? 0;
            }
        }

        #endregion

        #region Positions

        public List<Position> GetAllPositions()
        {
            using (var db = new PayrollDbContext())
            {
                return db.Positions.OrderBy(p => p.PositionName).ToList();
            }
        }

        public void AddPosition(Position position)
        {
            using (var db = new PayrollDbContext())
            {
                db.Positions.Add(position);
                db.SaveChanges();
            }
        }

        public void DeletePosition(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var position = db.Positions.Find(id);
                if (position != null)
                {
                    db.Positions.Remove(position);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Departments

        public List<Department> GetAllDepartments()
        {
            using (var db = new PayrollDbContext())
            {
                return db.Departments.OrderBy(d => d.DepartmentName).ToList();
            }
        }

        public void AddDepartment(Department department)
        {
            using (var db = new PayrollDbContext())
            {
                db.Departments.Add(department);
                db.SaveChanges();
            }
        }

        public void DeleteDepartment(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var department = db.Departments.Find(id);
                if (department != null)
                {
                    db.Departments.Remove(department);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Timesheet

        public List<Timesheet> GetTimesheets(int? employeeId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var db = new PayrollDbContext())
            {
                var query = db.Timesheets.Include(t => t.Employee).AsQueryable();

                if (employeeId.HasValue)
                    query = query.Where(t => t.EmployeeID == employeeId.Value);
                if (startDate.HasValue)
                    query = query.Where(t => t.DateWorked >= startDate.Value);
                if (endDate.HasValue)
                    query = query.Where(t => t.DateWorked <= endDate.Value);

                return query.OrderByDescending(t => t.DateWorked).ToList();
            }
        }

        public void AddTimesheet(Timesheet timesheet)
        {
            using (var db = new PayrollDbContext())
            {
                db.Timesheets.Add(timesheet);
                db.SaveChanges();
            }
        }

        public void DeleteTimesheet(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var timesheet = db.Timesheets.Find(id);
                if (timesheet != null)
                {
                    db.Timesheets.Remove(timesheet);
                    db.SaveChanges();
                }
            }
        }

        public decimal GetWorkedHoursForPeriod(int employeeId, int year, int month)
        {
            using (var db = new PayrollDbContext())
            {
                return db.Timesheets
                    .Where(t => t.EmployeeID == employeeId &&
                                t.DateWorked.Year == year &&
                                t.DateWorked.Month == month)
                    .Sum(t => (decimal?)t.Hours) ?? 0;
            }
        }

        #endregion

        #region Production Calendar

        public List<ProductionCalendar> GetProductionCalendar(int? year = null)
        {
            using (var db = new PayrollDbContext())
            {
                var query = db.ProductionCalendars.AsQueryable();

                if (year.HasValue)
                    query = query.Where(pc => pc.Year == year.Value);

                return query.OrderByDescending(pc => pc.Year).ThenBy(pc => pc.Month).ToList();
            }
        }

        public decimal GetNormHours(int year, int month)
        {
            using (var db = new PayrollDbContext())
            {
                return db.ProductionCalendars
                    .FirstOrDefault(pc => pc.Year == year && pc.Month == month)?.WorkingHours ?? 168;
            }
        }

        public decimal GetNormHoursByPeriod(string period)
        {
            var parts = period.Split('-');
            return GetNormHours(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public void SaveProductionCalendar(int year, int month, decimal hours)
        {
            using (var db = new PayrollDbContext())
            {
                var existing = db.ProductionCalendars.FirstOrDefault(pc => pc.Year == year && pc.Month == month);

                if (existing != null)
                {
                    existing.WorkingHours = hours;
                }
                else
                {
                    db.ProductionCalendars.Add(new ProductionCalendar { Year = year, Month = month, WorkingHours = hours });
                }

                db.SaveChanges();
            }
        }

        #endregion

        #region Deductions

        public List<Deduction> GetAllDeductions()
        {
            using (var db = new PayrollDbContext())
            {
                return db.Deductions.ToList();
            }
        }

        public void AddDeduction(Deduction deduction)
        {
            using (var db = new PayrollDbContext())
            {
                db.Deductions.Add(deduction);
                db.SaveChanges();
            }
        }

        public void DeleteDeduction(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var deduction = db.Deductions.Find(id);
                if (deduction != null)
                {
                    db.Deductions.Remove(deduction);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Payroll

        public List<Payroll> GetPayrolls(string period = null)
        {
            using (var db = new PayrollDbContext())
            {
                var query = db.Payrolls
                    .Include(p => p.Employee)
                    .Include(p => p.PayrollDeductions)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(period))
                    query = query.Where(p => p.Period == period);

                return query.OrderBy(p => p.Employee.LastName).ToList();
            }
        }

        public Payroll GetPayrollForEmployee(int employeeId, string period)
        {
            using (var db = new PayrollDbContext())
            {
                return db.Payrolls
                    .Include(p => p.Employee)
                    .Include(p => p.PayrollDeductions)
                    .FirstOrDefault(p => p.EmployeeID == employeeId && p.Period == period);
            }
        }

        public List<PayrollDeduction> GetPayrollDeductions(int payrollId)
        {
            using (var db = new PayrollDbContext())
            {
                return db.PayrollDeductions
                    .Include(pd => pd.Deduction)
                    .Where(pd => pd.PayrollID == payrollId)
                    .ToList();
            }
        }

        public void CalculatePayrollForAll(string period)
        {
            var parts = period.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            decimal normHours = GetNormHours(year, month);

            foreach (var emp in GetAllEmployees())
            {
                decimal workedHours = GetWorkedHoursForPeriod(emp.EmployeeID, year, month);
                var existing = GetPayrollForEmployee(emp.EmployeeID, period);
                decimal bonus = existing?.Bonus ?? 0;
                decimal penalty = existing?.Penalty ?? 0;

                CalculateAndSavePayroll(emp, period, normHours, workedHours, bonus, penalty);
            }
        }

        private void CalculateAndSavePayroll(Employee emp, string period, decimal normHours, decimal workedHours, decimal bonus, decimal penalty)
        {
            decimal hourlyRate = normHours > 0 ? emp.BaseSalary / normHours : 0;
            decimal baseSalary = workedHours <= normHours
                ? hourlyRate * workedHours
                : emp.BaseSalary + (workedHours - normHours) * hourlyRate * 1.5m;

            decimal taxBase = Math.Max(0, baseSalary + bonus - penalty);
            decimal tax = taxBase * 0.13m;
            decimal netSalary = taxBase - tax;

            using (var db = new PayrollDbContext())
            {
                var payroll = db.Payrolls.FirstOrDefault(p => p.EmployeeID == emp.EmployeeID && p.Period == period);

                if (payroll != null)
                {
                    payroll.BaseSalary = baseSalary;
                    payroll.WorkedHours = workedHours;
                    payroll.Bonus = bonus;
                    payroll.Penalty = penalty;
                    payroll.NetSalary = netSalary;
                }
                else
                {
                    payroll = new Payroll
                    {
                        EmployeeID = emp.EmployeeID,
                        Period = period,
                        BaseSalary = baseSalary,
                        WorkedHours = workedHours,
                        Bonus = bonus,
                        Penalty = penalty,
                        NetSalary = netSalary
                    };
                    db.Payrolls.Add(payroll);
                }

                db.SaveChanges();
                SavePayrollDeductions(db, payroll.PayrollID, tax, penalty);
            }
        }

        public void UpdatePayrollBonusAndPenalty(int payrollId, decimal bonus, decimal penalty)
        {
            using (var db = new PayrollDbContext())
            {
                var payroll = db.Payrolls.Find(payrollId);
                if (payroll == null) return;

                decimal taxBase = Math.Max(0, payroll.BaseSalary + bonus - penalty);
                decimal tax = taxBase * 0.13m;

                payroll.Bonus = bonus;
                payroll.Penalty = penalty;
                payroll.NetSalary = taxBase - tax;

                db.SaveChanges();
                SavePayrollDeductions(db, payrollId, tax, penalty);
            }
        }

        private void SavePayrollDeductions(PayrollDbContext db, int payrollId, decimal tax, decimal penalty)
        {
            var oldDeductions = db.PayrollDeductions.Where(pd => pd.PayrollID == payrollId);
            db.PayrollDeductions.RemoveRange(oldDeductions);

            db.PayrollDeductions.Add(new PayrollDeduction { PayrollID = payrollId, DeductionID = 1, Amount = tax });

            if (penalty > 0)
                db.PayrollDeductions.Add(new PayrollDeduction { PayrollID = payrollId, DeductionID = 2, Amount = penalty });

            db.SaveChanges();
        }

        #endregion

        #region Reports

        public decimal GetTotalPayrollForPeriod(string period)
        {
            using (var db = new PayrollDbContext())
            {
                return db.Payrolls.Where(p => p.Period == period).Sum(p => (decimal?)p.NetSalary) ?? 0;
            }
        }

        public int GetEmployeeCount()
        {
            using (var db = new PayrollDbContext())
            {
                return db.Employees.Count();
            }
        }

        #endregion

        #region Users

        public User Login(string username, string password)
        {
            using (var db = new PayrollDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            }
        }

        public List<User> GetAllUsers()
        {
            using (var db = new PayrollDbContext())
            {
                return db.Users.ToList();
            }
        }

        public void AddUser(User user)
        {
            using (var db = new PayrollDbContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            using (var db = new PayrollDbContext())
            {
                var user = db.Users.Find(id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
        }

        #endregion
    }
}
