using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLLab2.ViewModel.EditSelectedTitleViewModel;

namespace SQLLab2.ViewModel
{
    internal class EditSelectedAuthorViewModel : ViewModelBase
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public AuthorViewModel SelectedAuthorViewModel { get; set; }
        public DelegateCommand UpdateAuthorAsyncCommand { get; private set; }

        public EditSelectedAuthorViewModel(MainWindowViewModel mainWindowViewModel, bool newAuthor)
        {
            MainWindowViewModel = mainWindowViewModel;

            if (!newAuthor && mainWindowViewModel.SelectedAuthor != null)
            {
                SelectedAuthorViewModel = mainWindowViewModel.SelectedAuthor;
            }
            else
            {
                SelectedAuthorViewModel = new AuthorViewModel(new Author());
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

            var originalAuthor = await db.Authors
                .Where(a => a.Id == SelectedAuthorViewModel.Author.Id)
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

            try
            {
                await db.SaveChangesAsync();

                UpdateMainWindowAuthorsCollection(originalAuthor, isNewAuthor);
            }

            catch (DbUpdateException ex)
            {
                MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void SaveChangesToAuthor(Author author)
        {
            if (SelectedAuthorViewModel == null || author == null)
                throw new ArgumentNullException(nameof(SelectedAuthorViewModel));

            author.FirstName = SelectedAuthorViewModel.FirstName;
            author.LastName = SelectedAuthorViewModel.LastName;
            author.BirthDate = SelectedAuthorViewModel.BirthDate;
            author.DeathDate = SelectedAuthorViewModel.DeathDate;
        }
        private void UpdateMainWindowAuthorsCollection(Author updatedAuthor, bool isNewAuthor)
        {
            var updatedAuthorViewModel = new AuthorViewModel(updatedAuthor);
            if (isNewAuthor)
            {
                MainWindowViewModel.Authors.Add(updatedAuthorViewModel);
            }
            else
            {
                var existingAuthorViewModel = MainWindowViewModel.Authors
                    .FirstOrDefault(a => a.Id == updatedAuthor.Id);

                if (existingAuthorViewModel != null)
                {
                    existingAuthorViewModel.FirstName = updatedAuthor.FirstName;
                    existingAuthorViewModel.LastName = updatedAuthor.LastName;
                    existingAuthorViewModel.BirthDate = updatedAuthor.BirthDate;
                    existingAuthorViewModel.DeathDate = updatedAuthor.DeathDate;

                    var index = MainWindowViewModel.Authors.IndexOf(existingAuthorViewModel);
                    MainWindowViewModel.Authors[index] = updatedAuthorViewModel;
                }
            }
        }
    }
}
