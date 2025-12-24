using System.Windows;
using System.Windows.Controls;
using PayrollSystem.Models;
using PayrollSystem.Views;

namespace PayrollSystem
{
    public partial class MainWindow : Window
    {
        public static User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MenuList.SelectedIndex = 0;
        }

        public void SetUser(User user)
        {
            CurrentUser = user;
            txtCurrentUser.Text = $"Пользователь: {user.Username} ({user.Role})";
            
            if (user.Role != "Admin")
            {
                menuAdmin.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuList.SelectedItem is ListBoxItem item)
            {
                string tag = item.Tag?.ToString();
                
                switch (tag)
                {
                    case "Employees":
                        MainFrame.Navigate(new EmployeesPage());
                        break;
                    case "Timesheet":
                        MainFrame.Navigate(new TimesheetPage());
                        break;
                    case "Payroll":
                        MainFrame.Navigate(new PayrollPage());
                        break;
                    case "Deductions":
                        MainFrame.Navigate(new DeductionsPage());
                        break;
                    case "Reports":
                        MainFrame.Navigate(new ReportsPage());
                        break;
                    case "Admin":
                        MainFrame.Navigate(new AdminPage());
                        break;
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
