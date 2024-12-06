using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<StoreSupply> _storeOneSupply;
        private ObservableCollection<StoreSupply> _storeTwoSupply;
        private ObservableCollection<StoreSupply> _storeThreeSupply;
        private List<Book> _books;
        private StoreSupply _selectedStoreSupply;
        private Book _selectedBook;

        public ObservableCollection<StoreSupply> StoreOneSupply
        {
            get => _storeOneSupply;
            set 
            { 
                _storeOneSupply = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<StoreSupply> StoreTwoSupply
        {
            get => _storeTwoSupply;
            set
            {
                _storeTwoSupply = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<StoreSupply> StoreThreeSupply
        {
            get => _storeThreeSupply;
            set
            {
                _storeThreeSupply = value;
                RaisePropertyChanged();
            }
        }

        public List<Book> Books
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

        public event Action<string> CreateDialogRequested;
        public MainWindowViewModel()
        {
            InitializeCommands();
            using var db = new BookstoreContext();

            try
            {
                StoreOneSupply = new ObservableCollection<StoreSupply>(
                db.StoreSupplies.Where(s => s.StoreId == 1).ToList());
                StoreTwoSupply = new ObservableCollection<StoreSupply>(
                    db.StoreSupplies.Where(s => s.StoreId == 2).ToList());
                StoreThreeSupply = new ObservableCollection<StoreSupply>(
                    db.StoreSupplies.Where(s => s.StoreId == 3).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching store supplies: {ex.Message}");
            }

            Books = GetBooks();
        }
        private void InitializeCommands()
        {
            CreateNewDialogCommand = new DelegateCommand(CreateNewDialog);
            SubtractSupplyCommand = new DelegateCommand(SubtractSupply);
            AddSupplyCommand = new DelegateCommand(AddSupply);
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

        private List<Book> GetBooks()   
        {
            using var db = new BookstoreContext();
            var books = db.StoreSupplies
                .Select(s => s.IsbnNavigation)
                .GroupBy(b => b.Isbn)
                .Select(g => g.First())
                .ToList();

            return books;
        }
    }
}
