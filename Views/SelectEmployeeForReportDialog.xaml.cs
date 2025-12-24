using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using PayrollSystem.Models;

namespace PayrollSystem.Views
{
    public partial class SelectEmployeeForReportDialog : Window
    {
        public Payroll SelectedPayroll { get; private set; }

        public SelectEmployeeForReportDialog(List<Payroll> payrolls)
        {
            InitializeComponent();
            dgEmployees.ItemsSource = payrolls;
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
            if (dgEmployees.SelectedItem is Payroll p)
            {
                SelectedPayroll = p;
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
