using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    internal class RemoveOrderViewModel
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public Order OrderToDelete { get; set; }
        public DelegateCommand DeleteOrderAsyncCommand { get; private set; }

        public RemoveOrderViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            OrderToDelete = MainWindowViewModel.SelectedOrder.Order;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteOrderAsyncCommand = new DelegateCommand(async obj => await DeleteOrderAsync(obj));
        }

        private async Task DeleteOrderAsync(object obj)
        {
            using var db = new BookstoreContext();

            db.Remove(OrderToDelete);

            await db.SaveChangesAsync();
            await MainWindowViewModel.RefreshCustomersAsync();
        }
    }
}
