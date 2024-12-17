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
        private ObservableCollection<StoreSupply> _storeSupply;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Genre> _genres;
        private ObservableCollection<Book> _books;
        private ObservableCollection<Customer> _customers;
        private StoreSupply _selectedStoreSupply;
        private Book _selectedBook;
        private Author _selectedAuthor;
        private Genre _selectedGenre;
        private Customer _selectedCustomer;
        private Order _selectedOrder;

        public ObservableCollection<StoreSupply> StoreSupply
        {
            get => _storeSupply;
            set
            {
                _storeSupply = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set
            { 
                _authors = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            { 
                _genres = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            { 
                _books = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Publisher> Publishers { get; set; }

        public StoreSupply SelectedStoreSupply
        {
            get => _selectedStoreSupply;
            set
            {
                _selectedStoreSupply = value;
                RaisePropertyChanged();

                UpdateSelectedBookAsync();
            }
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged();
            }
        }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged();
            }
        }

        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                RaisePropertyChanged();
            }
        }

        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                _selectedGenre = value;
                RaisePropertyChanged();
            }
        }

        public Book SelectedBook
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
            SelectedBook = await db.Books
                           .Include(b => b.Authors)
                           .Include(b => b.Publisher)
                           .Include(b => b.Genres)
                           .SingleOrDefaultAsync(b => b.Isbn == SelectedStoreSupply.Isbn);
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

                StoreSupply = new ObservableCollection<StoreSupply>(storeSupplies);
            }
        }

        private async void AddSupply(object obj)
        {
            if (SelectedStoreSupply != null)
            {
                using var db = new BookstoreContext();

                var storeSupplies = await db.StoreSupplies
                    .Where(s => s == SelectedStoreSupply)
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
                    .Where(s => s == SelectedStoreSupply)
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
                Authors = new ObservableCollection<Author>(
                    await db.Authors
                    .ToListAsync());
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
                Books = new ObservableCollection<Book>(
                    await db.Books.Include(b => b.Authors)
                    .Include(b => b.Publisher)
                    .Include(b => b.Genres)
                    .ToListAsync());

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
                Genres = new ObservableCollection<Genre>(
                    await db.Genres
                    .ToListAsync());
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
                Customers = new ObservableCollection<Customer>(
                    await db.Customers
                          .Include(c => c.Orders)
                          .ThenInclude(o => o.OrderBookJts)
                          .ThenInclude(ob => ob.BookIsbnNavigation)
                          .ToListAsync());
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke($"Error loading customers: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
