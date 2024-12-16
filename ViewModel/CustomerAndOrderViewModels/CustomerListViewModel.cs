using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class CustomerListViewModel : ViewModelBase
    {
        private ObservableCollection<Customer> customers;

        public MainWindowViewModel MainWindowViewModel { get; set; }
        public ObservableCollection<Customer> Customers
        {
            get => customers;
            set
            { 
                customers = value;
                RaisePropertyChanged();
            }
        }
        public Customer SelectedCustomer { get; set; }
        public CustomerListViewModel()
        {
            MainWindowViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel;
            Task.Run(async () => await InitializeDataAsync());
        }
        private async Task InitializeDataAsync()
        {
            try
            {
                await RefreshCustomersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching store information: {ex.Message}");
            }
        }
        public async Task RefreshCustomersAsync()
        {
            using var db = new BookstoreContext();
            Customers = new ObservableCollection<Customer>(
                await db.Customers
                      .Include(c => c.Orders)
                      .ThenInclude(o => o.OrderBookJts)
                      .ThenInclude(ob => ob.BookIsbnNavigation)
                      .ToListAsync());
        }
    }
}
