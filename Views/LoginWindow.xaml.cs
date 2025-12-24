using System;
using System.Windows;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseService _db;

        public LoginWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите имя пользователя и пароль";
                return;
            }

            try
            {
                var user = _db.Login(username, password);
                
                if (user != null)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.SetUser(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    txtError.Text = "Неверное имя пользователя или пароль";
                }
            }
            catch (Exception ex)
            {
                txtError.Text = $"Ошибка подключения: {ex.Message}";
            }
        }
    }
}
