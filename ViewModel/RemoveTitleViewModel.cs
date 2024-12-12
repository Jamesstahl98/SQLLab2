using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class RemoveTitleViewModel
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public Book BookToDelete { get; set; }
        public DelegateCommand DeleteBookCommand { get; private set; }

        public RemoveTitleViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            BookToDelete = MainWindowViewModel.SelectedBook;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteBookCommand = new DelegateCommand(DeleteBook);
        }

        private void DeleteBook(object obj)
        {
            using var db = new BookstoreContext();

            db.Remove(BookToDelete);

            db.SaveChanges();
            MainWindowViewModel.RefreshBooks();
        }
    }
}
