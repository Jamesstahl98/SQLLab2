using SQLLab2.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLLab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            viewModel.CreateDialogRequested += OnCreateDialogRequested;
            viewModel.ChangeWindowStateRequested += OnChangeWindowStateRequested;
            viewModel.RequestExit += () => Application.Current.Shutdown();
        }

        private void OnCreateDialogRequested(string className)
        {
            if (string.IsNullOrEmpty(className))
            {
                return;
            }

            Type windowType = Type.GetType("SQLLab2.Dialogs." + className);

            if (windowType != null)
            {
                Window newWindow = (Window)Activator.CreateInstance(windowType);
                newWindow.Show();
            }
        }
        private void OnChangeWindowStateRequested()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
    }
}