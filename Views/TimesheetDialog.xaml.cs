using System;
using System.Windows;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class TimesheetDialog : Window
    {
        private readonly DatabaseService _db;

        public TimesheetDialog()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadEmployees();
            dpDate.SelectedDate = DateTime.Now;
            txtHours.Text = "8";
        }

        private void LoadEmployees()
        {
            cmbEmployee.ItemsSource = _db.GetAllEmployees();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtHours.Text, out decimal hours) || hours <= 0 || hours > 24)
            {
                MessageBox.Show("Введите корректное количество часов (1-24)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var ts = new Timesheet
                {
                    EmployeeID = (int)cmbEmployee.SelectedValue,
                    DateWorked = dpDate.SelectedDate ?? DateTime.Now,
                    Hours = hours
                };

                _db.AddTimesheet(ts);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
