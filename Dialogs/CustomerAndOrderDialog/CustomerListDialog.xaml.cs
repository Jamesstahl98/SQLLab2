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
            (DataContext as MainWindowViewModel).SelectedCustomer = null;
            (DataContext as MainWindowViewModel).SelectedOrder = null;
            if (e.NewValue is Customer customer)
            {
                (DataContext as MainWindowViewModel).SelectedCustomer = customer;
            }
            else if(e.NewValue is Order order)
            {
                (DataContext as MainWindowViewModel).SelectedOrder = order;
            }
            else if(e.NewValue is OrderBookJt orderBookJt)
            {
                (DataContext as MainWindowViewModel).SelectedOrder = orderBookJt.Order;
            }
        }
    }
}
