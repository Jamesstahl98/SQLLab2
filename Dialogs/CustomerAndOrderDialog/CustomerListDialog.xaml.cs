using SQLLab2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SQLLab2.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomerListDialog.xaml
    /// </summary>
    public partial class CustomerListDialog : Window
    {
        public CustomerListDialog()
        {
            InitializeComponent();
            DataContext = (App.Current.MainWindow as MainWindow).DataContext;
        }
        public void CloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var viewModel = (DataContext as MainWindowViewModel);

            viewModel.SelectedCustomer = null;
            viewModel.SelectedOrder = null;

            if (e.NewValue is CustomerViewModel customer)
            {
                viewModel.SelectedCustomer = customer;
            }
            else if(e.NewValue is OrderViewModel order)
            {
                viewModel.SelectedOrder = order;
                viewModel.SelectedCustomer = order.Customer;
            }
        }
    }
}
