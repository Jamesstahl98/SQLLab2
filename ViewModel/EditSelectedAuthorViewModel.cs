using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLLab2.ViewModel.EditSelectedTitleViewModel;

namespace SQLLab2.ViewModel
{
    internal class EditSelectedAuthorViewModel : ViewModelBase
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public Author SelectedAuthor { get; set; }
        public DelegateCommand UpdateAuthorAsyncCommand { get; private set; }

        public EditSelectedAuthorViewModel(MainWindowViewModel mainWindowViewModel, bool newAuthor)
        {
            MainWindowViewModel = mainWindowViewModel;

            if (!newAuthor && mainWindowViewModel.SelectedAuthor != null)
            {
                SelectedAuthor = new Author(mainWindowViewModel.SelectedAuthor);
            }
            else
            {
                SelectedAuthor = new Author();
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateAuthorAsyncCommand = new DelegateCommand(async obj => await (UpdateAuthorAsync(obj)));
        }

        private async Task UpdateAuthorAsync(object obj)
        {
            bool isNewAuthor = false;

            using var db = new BookstoreContext();

            var originalAuthor = await db.Authors.Where(a => a.Id == SelectedAuthor.Id)
                .FirstOrDefaultAsync();

            if (originalAuthor == null)
            {
                originalAuthor = new Author();
                isNewAuthor = true;
            }

            SaveChangesToAuthor(originalAuthor);

            if(isNewAuthor)
            {
                await db.Authors.AddAsync(originalAuthor);
            }

            await db.SaveChangesAsync();

            await MainWindowViewModel.RefreshAuthorsAsync();
            await MainWindowViewModel.RefreshBooksAsync();
        }

        private void SaveChangesToAuthor(Author author)
        {
            author.FirstName = SelectedAuthor.FirstName;
            author.LastName = SelectedAuthor.LastName;
            author.BirthDate = SelectedAuthor.BirthDate;
            author.DeathDate = SelectedAuthor.DeathDate;
        }
    }
}
