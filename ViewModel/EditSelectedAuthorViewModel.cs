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
        public DelegateCommand UpdateAuthorCommand { get; private set; }

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
            UpdateAuthorCommand = new DelegateCommand(UpdateAuthor);
        }

        private void UpdateAuthor(object obj)
        {
            bool isNewAuthor = false;

            using var db = new BookstoreContext();

            var originalAuthor = db.Authors.Where(a => a.Id == SelectedAuthor.Id)
                .FirstOrDefault();

            if (originalAuthor == null)
            {
                originalAuthor = new Author();
                isNewAuthor = true;
            }

            SaveChangesToAuthor(originalAuthor);

            if(isNewAuthor)
            {
                db.Authors.Add(originalAuthor);
            }

            db.SaveChanges();

            MainWindowViewModel.RefreshAuthors();
            MainWindowViewModel.RefreshBooks();
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
