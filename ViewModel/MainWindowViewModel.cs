using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SQLLab2.Commands;
using SQLLab2.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private StoreSupply _selectedStoreSupply;
        private Book _selectedBook;
        private Store _selectedStore;
        private ObservableCollection<StoreSupply> _storeSupply;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Book> _books;

        public Store SelectedStore
        {
            get => _selectedStore;
            set 
            { 
                _selectedStore = value;
                RaisePropertyChanged();
            }
        }
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
        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            { 
                _books = value;
                RaisePropertyChanged();
            }
        }

        public StoreSupply SelectedStoreSupply
        {
            get => _selectedStoreSupply;
            set
            {
                _selectedStoreSupply = value;
                RaisePropertyChanged();

                UpdateSelectedBook();
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

        private void UpdateSelectedBook()
        {
            if (SelectedStoreSupply == null)
            {
                SelectedBook = null;
                return;
            }

            using var db = new BookstoreContext();
            SelectedBook = db.Books
                           .Include(b => b.Authors)
                           .Include(b => b.Publisher)
                           .SingleOrDefault(b => b.Isbn == SelectedStoreSupply.Isbn);
        }

        public DelegateCommand CreateNewDialogCommand { get; private set; }
        public DelegateCommand SubtractSupplyCommand { get; private set; }
        public DelegateCommand AddSupplyCommand { get; private set; }
        public DelegateCommand ChangeStoreCommand { get; private set; }

        public event Action<string> CreateDialogRequested;
        public MainWindowViewModel()
        {
            InitializeCommands();
            using var db = new BookstoreContext();

            try
            {
                StoreSupply = new ObservableCollection<StoreSupply>(
                db.StoreSupplies.Where(s => s.StoreId == 1).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching store supplies: {ex.Message}");
            }

            RefreshBooks();
            Authors = GetAuthors();
        }
        private void InitializeCommands()
        {
            CreateNewDialogCommand = new DelegateCommand(CreateNewDialog);
            SubtractSupplyCommand = new DelegateCommand(SubtractSupply);
            AddSupplyCommand = new DelegateCommand(AddSupply);
            ChangeStoreCommand = new DelegateCommand(ChangeStore);
        }

        private void ChangeStore(object obj)
        {
            int parameterId = Convert.ToInt32(obj);
            if (parameterId is int storeId)
            {
                using var db = new BookstoreContext();
                var storeSupplies = db.StoreSupplies.Where(s => s.StoreId == storeId).ToList();

                StoreSupply = new ObservableCollection<StoreSupply>(storeSupplies);
            }
        }

        private void AddSupply(object obj)
        {
            if (SelectedStoreSupply != null)
            {
                SelectedStoreSupply.Amount++;
            }
        }

        private void SubtractSupply(object obj)
        {
            if (SelectedStoreSupply != null)
            {
                SelectedStoreSupply.Amount--;
            }
        }

        private void CreateNewDialog(object obj)
        {
            string className = obj as string;

            CreateDialogRequested?.Invoke(className);
        }

        private ObservableCollection<Author> GetAuthors()
        {
            using var db = new BookstoreContext();
            var authors = new ObservableCollection<Author>(
                db.Authors
                .ToList());

            return authors;
        }
        public void RefreshBooks()
        {
            using var db = new BookstoreContext();
            Books = new ObservableCollection<Book>(
                db.Books.Include(b => b.Authors)
                .Include(b => b.Publisher)
                .ToList());
        }
    }
}
