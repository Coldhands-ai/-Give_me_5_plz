using System;
using System.Windows;
using System.Windows.Controls;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class AdminPage : Page
    {
        private readonly DatabaseService _db;

        public AdminPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            InitializeCalendarComboBoxes();
            cmbRole.SelectedIndex = 1;
            LoadAllData();
        }

        private void InitializeCalendarComboBoxes()
        {
            for (int year = 2020; year <= DateTime.Now.Year + 2; year++)
            {
                cmbCalendarYear.Items.Add(year);
            }
            cmbCalendarYear.SelectedItem = DateTime.Now.Year;

            cmbCalendarMonth.Items.Add(new { Value = 1, Name = "Январь" });
            cmbCalendarMonth.Items.Add(new { Value = 2, Name = "Февраль" });
            cmbCalendarMonth.Items.Add(new { Value = 3, Name = "Март" });
            cmbCalendarMonth.Items.Add(new { Value = 4, Name = "Апрель" });
            cmbCalendarMonth.Items.Add(new { Value = 5, Name = "Май" });
            cmbCalendarMonth.Items.Add(new { Value = 6, Name = "Июнь" });
            cmbCalendarMonth.Items.Add(new { Value = 7, Name = "Июль" });
            cmbCalendarMonth.Items.Add(new { Value = 8, Name = "Август" });
            cmbCalendarMonth.Items.Add(new { Value = 9, Name = "Сентябрь" });
            cmbCalendarMonth.Items.Add(new { Value = 10, Name = "Октябрь" });
            cmbCalendarMonth.Items.Add(new { Value = 11, Name = "Ноябрь" });
            cmbCalendarMonth.Items.Add(new { Value = 12, Name = "Декабрь" });
            cmbCalendarMonth.DisplayMemberPath = "Name";
            cmbCalendarMonth.SelectedValuePath = "Value";
            cmbCalendarMonth.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void LoadAllData()
        {
            LoadUsers();
            LoadDepartments();
            LoadPositions();
            LoadCalendar();
        }

        #region Production Calendar

        private void LoadCalendar()
        {
            try
            {
                int? year = cmbCalendarYear.SelectedItem as int?;
                dgCalendar.ItemsSource = _db.GetProductionCalendar(year);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки календаря: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbCalendarYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCalendar();
        }

        private void BtnSaveCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCalendarYear.SelectedItem == null || cmbCalendarMonth.SelectedValue == null)
            {
                MessageBox.Show("Выберите год и месяц", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNormHours.Text, out decimal hours) || hours <= 0)
            {
                MessageBox.Show("Введите корректное количество часов", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int year = (int)cmbCalendarYear.SelectedItem;
                int month = (int)cmbCalendarMonth.SelectedValue;
                _db.SaveProductionCalendar(year, month, hours);
                MessageBox.Show("Сохранено!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNormHours.Text = "";
                LoadCalendar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Users

        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource = _db.GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Заполните все поля", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _db.AddUser(new User { Username = username, Password = password, Role = role });
                txtUsername.Text = "";
                txtPassword.Text = "";
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                if (user.UserID == 1)
                {
                    MessageBox.Show("Нельзя удалить главного администратора", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить пользователя {user.Username}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeleteUser(user.UserID);
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Departments

        private void LoadDepartments()
        {
            try
            {
                dgDepartments.ItemsSource = _db.GetAllDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отделов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            string name = txtDepartmentName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название отдела", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _db.AddDepartment(new Department { DepartmentName = name });
                txtDepartmentName.Text = "";
                LoadDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (dgDepartments.SelectedItem is Department dept)
            {
                if (MessageBox.Show($"Удалить отдел \"{dept.DepartmentName}\"?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeleteDepartment(dept.DepartmentID);
                        LoadDepartments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}\n\nВозможно, есть сотрудники в этом отделе.", 
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите отдел", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Positions

        private void LoadPositions()
        {
            try
            {
                dgPositions.ItemsSource = _db.GetAllPositions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки должностей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddPosition_Click(object sender, RoutedEventArgs e)
        {
            string name = txtPositionName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название должности", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPositionSalary.Text, out decimal salary))
            {
                MessageBox.Show("Введите корректный оклад", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _db.AddPosition(new Position { PositionName = name, BaseSalary = salary });
                txtPositionName.Text = "";
                txtPositionSalary.Text = "";
                LoadPositions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeletePosition_Click(object sender, RoutedEventArgs e)
        {
            if (dgPositions.SelectedItem is Position pos)
            {
                if (MessageBox.Show($"Удалить должность \"{pos.PositionName}\"?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeletePosition(pos.PositionID);
                        LoadPositions();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}\n\nВозможно, есть сотрудники на этой должности.", 
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите должность", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion
    }
}
