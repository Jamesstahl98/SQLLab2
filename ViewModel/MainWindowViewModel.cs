using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SQLLab2.Commands;
using SQLLab2.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SQLLab2.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<StoreSupplyViewModel> _storeSupply;
        private ObservableCollection<AuthorViewModel> _authors;
        private ObservableCollection<GenreViewModel> _genres;
        private ObservableCollection<BookViewModel> _books;
        private ObservableCollection<CustomerViewModel> _customers;
        private ObservableCollection<OrderViewModel> _orders;
        private StoreSupplyViewModel _selectedStoreSupply;
        private BookViewModel _selectedBook;
        private AuthorViewModel _selectedAuthor;
        private GenreViewModel _selectedGenre;
        private CustomerViewModel _selectedCustomer;
        private OrderViewModel _selectedOrder;

        public ObservableCollection<StoreSupplyViewModel> StoreSupply
        {
            get => _storeSupply;
            set
            {
                _storeSupply = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<AuthorViewModel> Authors
        {
            get => _authors;
            set
            { 
                _authors = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<GenreViewModel> Genres
        {
            get => _genres;
            set
            { 
                _genres = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BookViewModel> Books
        {
            get => _books;
            set
            { 
                _books = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<CustomerViewModel> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<OrderViewModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Publisher> Publishers { get; set; }

        public StoreSupplyViewModel SelectedStoreSupply
        {
            get => _selectedStoreSupply;
            set
            {
                _selectedStoreSupply = value;
                RaisePropertyChanged();

                UpdateSelectedBookAsync();
            }
        }

        public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged();
            }
        }
        public OrderViewModel SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged();
            }
        }

        public AuthorViewModel SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                RaisePropertyChanged();
            }
        }

        public GenreViewModel SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                _selectedGenre = value;
                RaisePropertyChanged();
            }
        }

        public BookViewModel SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                RaisePropertyChanged();
            }
        }

        private async Task UpdateSelectedBookAsync()
        {
            if (SelectedStoreSupply == null)
            {
                SelectedBook = null;
                return;
            }

            using var db = new BookstoreContext();
            var book = await db.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .Include(b => b.Genres)
                .SingleOrDefaultAsync(b => b.Isbn == SelectedStoreSupply.Isbn);

            SelectedBook = book != null ? new BookViewModel(book) : null;
        }

        public DelegateCommand CreateNewDialogCommand { get; private set; }
        public DelegateCommand SubtractSupplyCommand { get; private set; }
        public DelegateCommand AddSupplyCommand { get; private set; }
        public DelegateCommand ChangeStoreAsyncCommand { get; private set; }

        public event Action<string> CreateDialogRequested;
        public Action<string> ShowMessage { get; set; } = message => MessageBox.Show(message);
        public MainWindowViewModel()
        {
            InitializeCommands();
            Task.Run(async () => await InitializeDataAsync());
        }

        private async Task InitializeDataAsync()
        {
            await ChangeStoreAsync(1);
            await RefreshBooksAsync();
            await RefreshPublishersAsync();
            await RefreshAuthorsAsync();
            await RefreshGenresAsync();
            await RefreshCustomersAsync();
        }

        private void InitializeCommands()
        {
            CreateNewDialogCommand = new DelegateCommand(CreateNewDialog);
            SubtractSupplyCommand = new DelegateCommand(SubtractSupply);
            AddSupplyCommand = new DelegateCommand(AddSupply);
            ChangeStoreAsyncCommand = new DelegateCommand(async obj => await ChangeStoreAsync(obj));
        }

        private async Task ChangeStoreAsync(object obj)
        {
            int parameterId = Convert.ToInt32(obj);
            if (parameterId is int storeId)
            {
                using var db = new BookstoreContext();

                var storeSupplies = await db.StoreSupplies
                    .Where(s => s.StoreId == storeId)
                    .Include(ss => ss.IsbnNavigation)
                    .ThenInclude(b => b.Authors)
                    .ToListAsync();

                StoreSupply = new ObservableCollection<StoreSupplyViewModel>(
                    storeSupplies.Select(s => new StoreSupplyViewModel(s))
                );
            }
        }

        private async void AddSupply(object obj)
        {
            if (SelectedStoreSupply != null)
            {
                using var db = new BookstoreContext();

                var storeSupplies = await db.StoreSupplies
                    .Where(s => s.StoreId == SelectedStoreSupply.StoreId 
                    && s.Isbn == SelectedStoreSupply.Isbn)
                    .FirstOrDefaultAsync();

                storeSupplies.Amount++;

                await db.SaveChangesAsync();

                SelectedStoreSupply.Amount = storeSupplies.Amount;
            }
        }

        private async void SubtractSupply(object obj)
        {
            if (SelectedStoreSupply != null && SelectedStoreSupply.Amount > 0)
            {
                using var db = new BookstoreContext();

                var storeSupplies = await db.StoreSupplies
                    .Where(s => s.StoreId == SelectedStoreSupply.StoreId
                    && s.Isbn == SelectedStoreSupply.Isbn)
                    .FirstOrDefaultAsync();

                storeSupplies.Amount--;

                await db.SaveChangesAsync();

                SelectedStoreSupply.Amount = storeSupplies.Amount;
            }
        }

        private void CreateNewDialog(object obj)
        {
            string className = obj as string;

            CreateDialogRequested?.Invoke(className);
        }
        public async Task AddBookToStoreSuppliesAsync(Book book)
        {
            using var db = new BookstoreContext();

            var storeCount = db.Stores.Count();

            for (int i = 1; i <= storeCount; i++)
            {
                var store = await db.Stores
                    .Where(s => s.Id == i)
                    .Include(s => s.StoreSupplies)
                    .SingleOrDefaultAsync();

                store.StoreSupplies.Add(new StoreSupply() { Isbn = book.Isbn, Amount = 0, StoreId = i });
            }

            await db.SaveChangesAsync();
        }
        public async Task RefreshAuthorsAsync()
        {
            try
            {
                using var db = new BookstoreContext();
                var authors = await db.Authors.ToListAsync();

                Authors = new ObservableCollection<AuthorViewModel>(
                    authors.Select(a => new AuthorViewModel(a))
                );
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Error loading authors: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        public async Task RefreshBooksAsync()
        {
            try
            {
                using var db = new BookstoreContext();
                var books = await db.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Publisher)
                    .Include(b => b.Genres)
                    .ToListAsync();

                Books = new ObservableCollection<BookViewModel>(
                    books.Select(b => new BookViewModel(b))
                );

                await ChangeStoreAsync(1);
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Error loading books: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        public async Task RefreshPublishersAsync()
        {
            try
            {
                using var db = new BookstoreContext();
                Publishers = new ObservableCollection<Publisher>(
                    await db.Publishers
                    .ToListAsync());
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Error loading publishers: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        public async Task RefreshGenresAsync()
        {
            try
            {
                using var db = new BookstoreContext();
                var genres = new ObservableCollection<Genre>(
                    await db.Genres
                    .ToListAsync());

                Genres = new ObservableCollection<GenreViewModel>(
                    genres.Select(g => new GenreViewModel(g))
                );
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Error loading genres: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
        public async Task RefreshCustomersAsync()
        {
            try
            {
                using var db = new BookstoreContext();
                var customers = new ObservableCollection<Customer>(
                    await db.Customers
                          .Include(c => c.Orders)
                          .ThenInclude(o => o.OrderBookJts)
                          .ThenInclude(ob => ob.BookIsbnNavigation)
                          .ToListAsync());
                Customers = new ObservableCollection<CustomerViewModel>(
                    customers.Select(c => new CustomerViewModel(c)));

                var orders = new ObservableCollection<Order>(
                    db.Orders);
                Orders = new ObservableCollection<OrderViewModel>(
                    orders.Select(o => new OrderViewModel(o)));
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Error loading customers: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
