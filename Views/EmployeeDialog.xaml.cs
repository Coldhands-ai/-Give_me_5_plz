using System;
using System.Windows;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class EmployeeDialog : Window
    {
        private readonly DatabaseService _db;
        private readonly Employee _employee;
        private readonly bool _isEdit;

        public EmployeeDialog(Employee employee = null)
        {
            InitializeComponent();
            _db = new DatabaseService();
            _employee = employee;
            _isEdit = employee != null;

            LoadComboBoxes();

            if (_isEdit)
            {
                txtTitle.Text = "Редактирование сотрудника";
                FillForm();
            }
            else
            {
                dpBirthDate.SelectedDate = DateTime.Now.AddYears(-25);
                dpHireDate.SelectedDate = DateTime.Now;
            }
        }

        private void LoadComboBoxes()
        {
            cmbPosition.ItemsSource = _db.GetAllPositions();
            cmbDepartment.ItemsSource = _db.GetAllDepartments();
        }

        private void FillForm()
        {
            txtLastName.Text = _employee.LastName;
            txtFirstName.Text = _employee.FirstName;
            txtMiddleName.Text = _employee.MiddleName;
            dpBirthDate.SelectedDate = _employee.BirthDate;
            dpHireDate.SelectedDate = _employee.HireDate;
            cmbPosition.SelectedValue = _employee.PositionID;
            cmbDepartment.SelectedValue = _employee.DepartmentID;
            txtSalary.Text = _employee.BaseSalary.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbPosition.SelectedValue == null)
            {
                MessageBox.Show("Выберите должность", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbDepartment.SelectedValue == null)
            {
                MessageBox.Show("Выберите отдел", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtSalary.Text, out decimal salary))
            {
                MessageBox.Show("Введите корректный оклад", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var emp = new Employee
                {
                    LastName = txtLastName.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim(),
                    BirthDate = dpBirthDate.SelectedDate ?? DateTime.Now,
                    HireDate = dpHireDate.SelectedDate ?? DateTime.Now,
                    PositionID = (int)cmbPosition.SelectedValue,
                    DepartmentID = (int)cmbDepartment.SelectedValue,
                    BaseSalary = salary
                };

                if (_isEdit)
                {
                    emp.EmployeeID = _employee.EmployeeID;
                    _db.UpdateEmployee(emp);
                }
                else
                {
                    _db.AddEmployee(emp);
                }

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
