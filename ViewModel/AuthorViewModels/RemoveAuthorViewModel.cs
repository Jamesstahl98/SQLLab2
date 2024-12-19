using Microsoft.EntityFrameworkCore;
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
        public AuthorViewModel AuthorViewModelToDelete { get; set; }
        public DelegateCommand DeleteAuthorAsyncCommand { get; private set; }

        public RemoveAuthorViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            AuthorViewModelToDelete = MainWindowViewModel.SelectedAuthor;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteAuthorAsyncCommand = new DelegateCommand(async obj => await DeleteAuthorAsync(obj));
        }

        private async Task DeleteAuthorAsync(object obj)
        {
            using var db = new BookstoreContext();

            var authorToDelete = await db.Authors
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(a => a.Id == AuthorViewModelToDelete.Author.Id);

            if (authorToDelete != null)
            {
                db.Authors.Remove(authorToDelete);
                await db.SaveChangesAsync();
            }

            MainWindowViewModel.Authors.Remove(AuthorViewModelToDelete);
        }
    }
}
