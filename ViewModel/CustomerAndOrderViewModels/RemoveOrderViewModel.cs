using Microsoft.EntityFrameworkCore;
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
        public OrderViewModel OrderViewModelToDelete { get; set; }
        public DelegateCommand DeleteOrderAsyncCommand { get; private set; }

        public RemoveOrderViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            OrderViewModelToDelete = MainWindowViewModel.SelectedOrder;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteOrderAsyncCommand = new DelegateCommand(async obj => await DeleteOrderAsync(obj));
        }

        private async Task DeleteOrderAsync(object obj)
        {
            using var db = new BookstoreContext();

            var orderToDelete = await db.Orders
                .Include(o => o.OrderBookJts)
                .FirstOrDefaultAsync(o => o.Id == OrderViewModelToDelete.Order.Id);

            if (orderToDelete != null)
            {
                db.Remove(orderToDelete);
                await db.SaveChangesAsync();

                var customerViewModel = MainWindowViewModel.Customers
                    .FirstOrDefault(c => c.Id == OrderViewModelToDelete.CustomerId);

                if (customerViewModel != null)
                {
                    customerViewModel.Orders.Remove(OrderViewModelToDelete);
                }
            }
        }
    }
}
