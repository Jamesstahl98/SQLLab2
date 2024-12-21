using Microsoft.EntityFrameworkCore;
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
        public BookViewModel BookViewModelToDelete { get; set; }
        public DelegateCommand DeleteBookAsyncCommand { get; private set; }

        public RemoveTitleViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            BookViewModelToDelete = MainWindowViewModel.SelectedBook;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteBookAsyncCommand = new DelegateCommand(async obj => await DeleteBookAsync(obj));
        }

        private async Task DeleteBookAsync(object obj)
        {
            using var db = new BookstoreContext();

            var bookToDelete = await db.Books
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(b => b.Isbn == BookViewModelToDelete.Book.Isbn);

            if (bookToDelete != null)
            {
                db.Books.Remove(bookToDelete);
                await db.SaveChangesAsync();
            }

            MainWindowViewModel.Books.Remove(BookViewModelToDelete);
            MainWindowViewModel.ChangeStoreAsync(1);
        }
    }
}
