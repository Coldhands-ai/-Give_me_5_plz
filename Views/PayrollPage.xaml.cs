using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class PayrollPage : Page
    {
        private readonly DatabaseService _db;
        private List<Payroll> _allPayrolls;

        public PayrollPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            InitializeComboBoxes();
        }

        private void InitializeComboBoxes()
        {
            cmbMonth.Items.Add(new { Value = "01", Name = "Январь" });
            cmbMonth.Items.Add(new { Value = "02", Name = "Февраль" });
            cmbMonth.Items.Add(new { Value = "03", Name = "Март" });
            cmbMonth.Items.Add(new { Value = "04", Name = "Апрель" });
            cmbMonth.Items.Add(new { Value = "05", Name = "Май" });
            cmbMonth.Items.Add(new { Value = "06", Name = "Июнь" });
            cmbMonth.Items.Add(new { Value = "07", Name = "Июль" });
            cmbMonth.Items.Add(new { Value = "08", Name = "Август" });
            cmbMonth.Items.Add(new { Value = "09", Name = "Сентябрь" });
            cmbMonth.Items.Add(new { Value = "10", Name = "Октябрь" });
            cmbMonth.Items.Add(new { Value = "11", Name = "Ноябрь" });
            cmbMonth.Items.Add(new { Value = "12", Name = "Декабрь" });
            cmbMonth.DisplayMemberPath = "Name";
            cmbMonth.SelectedValuePath = "Value";
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            for (int year = 2020; year <= DateTime.Now.Year + 1; year++)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = DateTime.Now.Year;
        }

        private string GetSelectedPeriod()
        {
            if (cmbYear.SelectedItem == null || cmbMonth.SelectedValue == null)
                return null;
            return $"{cmbYear.SelectedItem}-{cmbMonth.SelectedValue}";
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            string period = GetSelectedPeriod();
            if (period == null)
            {
                MessageBox.Show("Выберите период", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _db.CalculatePayrollForAll(period);
                MessageBox.Show("Расчёт выполнен!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(period);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            string period = GetSelectedPeriod();
            if (period == null)
            {
                MessageBox.Show("Выберите период", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LoadData(period);
        }

        private void LoadData(string period)
        {
            try
            {
                _allPayrolls = _db.GetPayrolls(period);
                ApplyFilter();
                txtCalculationDetails.Text = "Выберите сотрудника для просмотра расчёта...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnResetSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allPayrolls == null) return;

            string search = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                dgPayroll.ItemsSource = _allPayrolls;
            }
            else
            {
                dgPayroll.ItemsSource = _allPayrolls.Where(p =>
                    p.EmployeeName.ToLower().Contains(search)
                ).ToList();
            }
        }

        private void DgPayroll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPayroll.SelectedItem is Payroll payroll)
            {
                ShowCalculationDetails(payroll);
            }
        }

        private void ShowCalculationDetails(Payroll payroll)
        {
            try
            {
                string period = GetSelectedPeriod();
                if (period == null) return;

                decimal normHours = _db.GetNormHoursByPeriod(period);
                decimal employeeSalary = _db.GetEmployeeSalary(payroll.EmployeeID);
                decimal hourlyRate = normHours > 0 ? employeeSalary / normHours : 0;
                decimal workedHours = payroll.WorkedHours;

                var sb = new StringBuilder();
                sb.AppendLine($"═══ Сотрудник: {payroll.EmployeeName} ═══");
                sb.AppendLine();
                sb.AppendLine($"► Часы: {workedHours:N1} (отработано) / {normHours:N0} (норма по произв. календарю)");
                sb.AppendLine($"► Оклад: {employeeSalary:N2} руб.");
                sb.AppendLine($"► Часовая ставка: {employeeSalary:N2} / {normHours:N0} = {hourlyRate:N2} руб./час");
                sb.AppendLine();

                decimal baseSalaryCalc;
                if (workedHours <= normHours)
                {
                    baseSalaryCalc = hourlyRate * workedHours;
                    sb.AppendLine("► Расчёт (часов ≤ нормы):");
                    sb.AppendLine($"   Начислено = {hourlyRate:N2} × {workedHours:N1} = {baseSalaryCalc:N2} руб.");
                }
                else
                {
                    decimal overtime = workedHours - normHours;
                    decimal overtimePay = overtime * hourlyRate * 1.5m;
                    baseSalaryCalc = employeeSalary + overtimePay;
                    sb.AppendLine("► Расчёт (есть переработка):");
                    sb.AppendLine($"   Основная часть: {employeeSalary:N2} руб. (полный оклад)");
                    sb.AppendLine($"   Переработка: {overtime:N1} час × {hourlyRate:N2} × 1.5 = {overtimePay:N2} руб.");
                    sb.AppendLine($"   Начислено = {employeeSalary:N2} + {overtimePay:N2} = {baseSalaryCalc:N2} руб.");
                }

                sb.AppendLine();
                sb.AppendLine($"► Премия: +{payroll.Bonus:N2} руб.");
                sb.AppendLine($"► Штраф: -{payroll.Penalty:N2} руб.");

                decimal taxBase = baseSalaryCalc + payroll.Bonus - payroll.Penalty;
                if (taxBase < 0) taxBase = 0;
                decimal tax = taxBase * 0.13m;

                sb.AppendLine();
                sb.AppendLine("► База для налога:");
                sb.AppendLine($"   {baseSalaryCalc:N2} + {payroll.Bonus:N2} - {payroll.Penalty:N2} = {taxBase:N2} руб.");
                sb.AppendLine();
                sb.AppendLine("► Удержания:");
                sb.AppendLine($"   НДФЛ 13%: {taxBase:N2} × 13% = {tax:N2} руб.");
                if (payroll.Penalty > 0)
                {
                    sb.AppendLine($"   Штраф: {payroll.Penalty:N2} руб.");
                }
                sb.AppendLine($"   Итого удержано: {tax + payroll.Penalty:N2} руб.");

                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════");
                decimal netCalc = taxBase - tax;
                sb.AppendLine($"► Формула: (Начислено + Премия - Штраф) - 13%");
                sb.AppendLine($"► К ВЫПЛАТЕ: {taxBase:N2} - {tax:N2} = {netCalc:N2} руб.");
                sb.AppendLine("═══════════════════════════════════════════════");

                txtCalculationDetails.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtCalculationDetails.Text = $"Ошибка получения данных: {ex.Message}";
            }
        }

        private void DgPayroll_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;

            if (e.Row.Item is Payroll payroll)
            {
                var textBox = e.EditingElement as TextBox;
                if (textBox == null) return;

                string columnHeader = e.Column.Header.ToString();
                decimal newValue;

                if (!decimal.TryParse(textBox.Text, out newValue))
                {
                    MessageBox.Show("Введите корректное число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }

                if (newValue < 0)
                {
                    MessageBox.Show("Значение не может быть отрицательным", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }

                try
                {
                    decimal bonus = payroll.Bonus;
                    decimal penalty = payroll.Penalty;

                    if (columnHeader == "Премия")
                        bonus = newValue;
                    else if (columnHeader == "Штраф")
                        penalty = newValue;

                    _db.UpdatePayrollBonusAndPenalty(payroll.PayrollID, bonus, penalty);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        string period = GetSelectedPeriod();
                        LoadData(period);

                        var updated = _allPayrolls?.FirstOrDefault(p => p.PayrollID == payroll.PayrollID);
                        if (updated != null)
                        {
                            dgPayroll.SelectedItem = updated;
                            ShowCalculationDetails(updated);
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                }
            }
        }
    }
}
