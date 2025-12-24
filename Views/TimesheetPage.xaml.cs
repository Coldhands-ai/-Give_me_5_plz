using System;
using System.Windows;
using System.Windows.Controls;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class TimesheetPage : Page
    {
        private readonly DatabaseService _db;

        public TimesheetPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadEmployees();
            LoadData();
        }

        private void LoadEmployees()
        {
            var employees = _db.GetAllEmployees();
            employees.Insert(0, new Employee { EmployeeID = 0, FirstName = "Все", LastName = "" });
            cmbEmployee.ItemsSource = employees;
            cmbEmployee.SelectedIndex = 0;
        }

        private void LoadData()
        {
            try
            {
                int? empId = null;
                if (cmbEmployee.SelectedValue != null && (int)cmbEmployee.SelectedValue > 0)
                    empId = (int)cmbEmployee.SelectedValue;

                dgTimesheet.ItemsSource = _db.GetTimesheets(empId, dpStartDate.SelectedDate, dpEndDate.SelectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            cmbEmployee.SelectedIndex = 0;
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            LoadData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TimesheetDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimesheet.SelectedItem is Timesheet ts)
            {
                var result = MessageBox.Show(
                    "Удалить эту запись?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeleteTimesheet(ts.TimesheetID);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
