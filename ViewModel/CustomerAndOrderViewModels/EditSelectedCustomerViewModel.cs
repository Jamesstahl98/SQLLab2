using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class EditSelectedCustomerViewModel
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public CustomerViewModel SelectedCustomer { get; set; }
        public DelegateCommand UpdateCustomerAsyncCommand { get; private set; }

        public EditSelectedCustomerViewModel(MainWindowViewModel mainWindowViewModel, bool newCustomer)
        {
            MainWindowViewModel = mainWindowViewModel;

            if (!newCustomer && mainWindowViewModel.SelectedCustomer != null)
            {
                SelectedCustomer = mainWindowViewModel.SelectedCustomer;
            }
            else
            {
                SelectedCustomer = new CustomerViewModel(new Customer());
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateCustomerAsyncCommand = new DelegateCommand(async obj => await (UpdateCustomerAsync(obj)));
        }

        private async Task UpdateCustomerAsync(object obj)
        {
            bool isNewCustomer = false;

            using var db = new BookstoreContext();

            var originalCustomer = await db.Customers.Where(a => a.Id == SelectedCustomer.Id)
                .FirstOrDefaultAsync();

            if (originalCustomer == null)
            {
                originalCustomer = new Customer();
                isNewCustomer = true;
            }

            SaveChangesToCustomer(originalCustomer);

            if (isNewCustomer)
            {
                await db.Customers.AddAsync(originalCustomer);
            }

            try
            {
                await db.SaveChangesAsync();

                await MainWindowViewModel.RefreshCustomersAsync();
            }

            catch (DbUpdateException ex)
            {
                MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void SaveChangesToCustomer(Customer customer)
        {
            customer.FirstName = SelectedCustomer.FirstName;
            customer.LastName = SelectedCustomer.LastName;
            customer.Birthdate = SelectedCustomer.Birthdate;
            customer.Email = SelectedCustomer.Email;
            customer.City = SelectedCustomer.City;
            customer.Address = SelectedCustomer.Address;
            customer.PostalCode = SelectedCustomer.PostalCode;
        }
    }
}
