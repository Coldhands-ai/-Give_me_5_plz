using System.Windows;
using System.Windows.Input;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem.Views
{
    public partial class SelectEmployeeDialog : Window
    {
        private readonly DatabaseService _db;
        public Employee SelectedEmployee { get; private set; }

        public SelectEmployeeDialog()
        {
            InitializeComponent();
            _db = new DatabaseService();
            dgEmployees.ItemsSource = _db.GetAllEmployees();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectAndClose();
        }

        private void DgEmployees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectAndClose();
        }

        private void SelectAndClose()
        {
            if (dgEmployees.SelectedItem is Employee emp)
            {
                SelectedEmployee = emp;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Выберите сотрудника", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
