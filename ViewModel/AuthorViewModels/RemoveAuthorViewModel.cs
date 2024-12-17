using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class RemoveAuthorViewModel
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public AuthorViewModel AuthorToDelete { get; set; }
        public DelegateCommand DeleteAuthorAsyncCommand { get; private set; }

        public RemoveAuthorViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            AuthorToDelete = MainWindowViewModel.SelectedAuthor;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteAuthorAsyncCommand = new DelegateCommand(async obj => await DeleteAuthorAsync(obj));
        }

        private async Task DeleteAuthorAsync(object obj)
        {
            using var db = new BookstoreContext();

            db.Remove(AuthorToDelete);

            await db.SaveChangesAsync();
            await MainWindowViewModel.RefreshAuthorsAsync();
        }
    }
}
