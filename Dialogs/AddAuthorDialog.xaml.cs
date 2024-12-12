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
    /// Interaction logic for AddAuthorDialog.xaml
    /// </summary>
    public partial class AddAuthorDialog : Window
    {
        public AddAuthorDialog()
        {
            InitializeComponent();
            DataContext = new EditSelectedAuthorViewModel((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel, true);
        }
        public void CloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
