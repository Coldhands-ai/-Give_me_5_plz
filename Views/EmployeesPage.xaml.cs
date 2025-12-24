using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class EmployeesPage : Page
    {
        private readonly DatabaseService _db;
        private List<Employee> _allEmployees;

        public EmployeesPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _allEmployees = _db.GetAllEmployees();
                dgEmployees.ItemsSource = _allEmployees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterEmployees();
        }

        private void FilterEmployees()
        {
            if (_allEmployees == null) return;

            string search = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                dgEmployees.ItemsSource = _allEmployees;
            }
            else
            {
                dgEmployees.ItemsSource = _allEmployees.Where(emp =>
                    emp.LastName.ToLower().Contains(search) ||
                    emp.FirstName.ToLower().Contains(search) ||
                    (emp.MiddleName != null && emp.MiddleName.ToLower().Contains(search))
                ).ToList();
            }
        }

        private void BtnResetSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            dgEmployees.ItemsSource = _allEmployees;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EmployeeDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee emp)
            {
                var dialog = new EmployeeDialog(emp);
                if (dialog.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee emp)
            {
                var result = MessageBox.Show(
                    $"Удалить сотрудника {emp.FullName}?", 
                    "Подтверждение",
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeleteEmployee(emp.EmployeeID);
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
                MessageBox.Show("Выберите сотрудника для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
