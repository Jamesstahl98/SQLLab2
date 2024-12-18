using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class RemoveCustomerViewModel
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public CustomerViewModel CustomerViewModelToDelete { get; set; }
        public DelegateCommand DeleteCustomerAsyncCommand { get; private set; }

        public RemoveCustomerViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            CustomerViewModelToDelete = MainWindowViewModel.SelectedCustomer;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteCustomerAsyncCommand = new DelegateCommand(async obj => await DeleteCustomerAsync(obj));
        }

        private async Task DeleteCustomerAsync(object obj)
        {
            using var db = new BookstoreContext();

            db.Remove(CustomerViewModelToDelete.Customer);

            await db.SaveChangesAsync();
            MainWindowViewModel.Customers.Remove(CustomerViewModelToDelete);
        }
    }
}
