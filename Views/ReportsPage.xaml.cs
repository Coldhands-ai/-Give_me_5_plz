using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class ReportsPage : Page
    {
        private readonly DatabaseService _db;
        private List<Payroll> _currentPayrolls;

        public ReportsPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            InitializeComboBoxes();
            LoadStatistics();
        }

        private void InitializeComboBoxes()
        {
            cmbReportMonth.Items.Add(new { Value = "01", Name = "Январь" });
            cmbReportMonth.Items.Add(new { Value = "02", Name = "Февраль" });
            cmbReportMonth.Items.Add(new { Value = "03", Name = "Март" });
            cmbReportMonth.Items.Add(new { Value = "04", Name = "Апрель" });
            cmbReportMonth.Items.Add(new { Value = "05", Name = "Май" });
            cmbReportMonth.Items.Add(new { Value = "06", Name = "Июнь" });
            cmbReportMonth.Items.Add(new { Value = "07", Name = "Июль" });
            cmbReportMonth.Items.Add(new { Value = "08", Name = "Август" });
            cmbReportMonth.Items.Add(new { Value = "09", Name = "Сентябрь" });
            cmbReportMonth.Items.Add(new { Value = "10", Name = "Октябрь" });
            cmbReportMonth.Items.Add(new { Value = "11", Name = "Ноябрь" });
            cmbReportMonth.Items.Add(new { Value = "12", Name = "Декабрь" });
            cmbReportMonth.DisplayMemberPath = "Name";
            cmbReportMonth.SelectedValuePath = "Value";
            cmbReportMonth.SelectedIndex = DateTime.Now.Month - 1;

            for (int year = 2020; year <= DateTime.Now.Year + 1; year++)
            {
                cmbReportYear.Items.Add(year);
            }
            cmbReportYear.SelectedItem = DateTime.Now.Year;
        }

        private string GetSelectedPeriod()
        {
            if (cmbReportYear.SelectedItem == null || cmbReportMonth.SelectedValue == null)
                return null;
            return $"{cmbReportYear.SelectedItem}-{cmbReportMonth.SelectedValue}";
        }

        private void LoadStatistics()
        {
            try
            {
                txtEmployeeCount.Text = _db.GetEmployeeCount().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string period = GetSelectedPeriod();
            if (period == null)
            {
                MessageBox.Show("Выберите период", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _currentPayrolls = _db.GetPayrolls(period);
                dgReport.ItemsSource = _currentPayrolls;

                decimal total = _currentPayrolls.Sum(p => p.NetSalary);
                txtReportTotal.Text = $"{total:N2} руб.";
                txtTotalPayroll.Text = $"{total:N2} руб.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPayrolls == null || !_currentPayrolls.Any())
            {
                MessageBox.Show("Сначала сформируйте отчёт", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SelectEmployeeForReportDialog(_currentPayrolls);
            if (dialog.ShowDialog() == true && dialog.SelectedPayroll != null)
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF файлы (*.pdf)|*.pdf",
                    FileName = $"Расчётный_лист_{dialog.SelectedPayroll.EmployeeName.Replace(" ", "_")}_{dialog.SelectedPayroll.Period}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    try
                    {
                        var deductions = _db.GetPayrollDeductions(dialog.SelectedPayroll.PayrollID);
                        ExportService.ExportPayslipToPdf(dialog.SelectedPayroll, deductions, saveDialog.FileName);
                        MessageBox.Show("Расчётный лист сохранён!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPayrolls == null || !_currentPayrolls.Any())
            {
                MessageBox.Show("Сначала сформируйте отчёт", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                FileName = $"Зарплатная_ведомость_{GetSelectedPeriod()}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    ExportService.ExportPayrollToExcel(_currentPayrolls, GetSelectedPeriod(), saveDialog.FileName);
                    MessageBox.Show("Ведомость сохранена!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
