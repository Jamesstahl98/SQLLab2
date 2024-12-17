using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class EditSelectedOrderViewModel : ViewModelBase
    {
        public class EditableBook : ViewModelBase
        {
            private BookViewModel _selectedBook;
            private int _amount;

            public BookViewModel SelectedBook
            {
                get => _selectedBook;
                set
                {
                    _selectedBook = value;
                    RaisePropertyChanged();
                }
            }

            public int Amount
            {
                get => _amount;
                set
                {
                    _amount = value;
                    RaisePropertyChanged();
                }
            }
        }

        private OrderViewModel _selectedOrder;
        private ObservableCollection<EditableBook> _editableBooks;

        public ObservableCollection<EditableBook> EditableBooks
        {
            get => _editableBooks;
            set
            {
                _editableBooks = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BookViewModel> AllBooks { get; set; }
        public MainWindowViewModel MainWindowViewModel { get; set; }

        public OrderViewModel SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand UpdateOrderAsyncCommand { get; private set; }
        public DelegateCommand AddBookCommand { get; private set; }
        public DelegateCommand RemoveBookCommand { get; private set; }

        public EditSelectedOrderViewModel(MainWindowViewModel mainWindowViewModel, bool newOrder)
        {
            MainWindowViewModel = mainWindowViewModel;

            AllBooks = MainWindowViewModel.Books;

            if (!newOrder && mainWindowViewModel.SelectedOrder != null)
            {
                SelectedOrder = mainWindowViewModel.SelectedOrder;

                EditableBooks = new ObservableCollection<EditableBook>(
                SelectedOrder.OrderBookJts.Select(book =>
                    new EditableBook
                    {
                        SelectedBook = AllBooks.FirstOrDefault(b => b.Isbn == book.BookIsbn) ?? AllBooks.FirstOrDefault()
                    })
                );
            }

            else
            {
                SelectedOrder = new OrderViewModel(new Order());

                EditableBooks = new ObservableCollection<EditableBook>();
            }

            SelectedOrder.CustomerId = MainWindowViewModel.SelectedCustomer.Id;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateOrderAsyncCommand = new DelegateCommand(async obj => await UpdateOrderAsync(obj));
            AddBookCommand = new DelegateCommand(AddBook);
            RemoveBookCommand = new DelegateCommand(RemoveBook);
        }

        private void RemoveBook(object obj)
        {
            EditableBooks.Remove((EditableBook)obj);
        }

        private void AddBook(object obj)
        {
            EditableBooks.Add(new EditableBook());
        }

        private async Task UpdateOrderAsync(object obj)
        {
            bool isNewOrder = false;

            using var db = new BookstoreContext();

            var originalOrder = await db.Orders
                .Include(b => b.OrderBookJts)
                .FirstOrDefaultAsync(o => o.Id == SelectedOrder.Id);

            if (originalOrder == null)
            {
                originalOrder = new Order();
                isNewOrder = true;
            }

            SaveChangesToOrder(originalOrder);

            if (isNewOrder)
            {
                try
                {
                    await db.Orders.AddAsync(originalOrder);
                }
                catch (Exception ex)
                {
                    MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
                    return;
                }
            }

            await AddBooksToOrderAsync(originalOrder, db);
            try
            {
                await db.SaveChangesAsync();

                await MainWindowViewModel.RefreshCustomersAsync();
            }
            catch (Exception ex)
            {
                MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void SaveChangesToOrder(Order order)
        {
            order.DateAndTimePlaced = SelectedOrder.DateAndTimePlaced;
            order.City = SelectedOrder.City;
            order.Address = SelectedOrder.Address;
            order.PostalCode = SelectedOrder.PostalCode;
            order.CustomerId = SelectedOrder.CustomerId;
        }

        private async Task AddBooksToOrderAsync(Order order, BookstoreContext db)
        {
            if (order.OrderBookJts != null)
            {
                order.OrderBookJts.Clear();
            }
            else
            {
                order.OrderBookJts = new ObservableCollection<OrderBookJt>();
            }

            foreach (var editableBook in EditableBooks)
            {
                var selectedBookViewModel = editableBook.SelectedBook;

                var trackedBook = await db.Books.FirstOrDefaultAsync(b => b.Isbn == selectedBookViewModel.Isbn);

                if (trackedBook != null)
                {
                    order.OrderBookJts.Add(new OrderBookJt
                    {
                        OrderId = order.Id,
                        BookIsbn = trackedBook.Isbn,
                        UnitPrice = trackedBook.Price,
                        Amount = 1
                    });
                }
                else
                {
                    db.Books.Attach(selectedBookViewModel.Book);

                    order.OrderBookJts.Add(new OrderBookJt
                    {
                        OrderId = order.Id,
                        BookIsbn = selectedBookViewModel.Isbn,
                        UnitPrice = selectedBookViewModel.Price,
                        Amount = 1
                    });
                }
            }
        }
    }
}
