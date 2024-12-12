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
        public Author AuthorToDelete { get; set; }
        public DelegateCommand DeleteAuthorCommand { get; private set; }

        public RemoveAuthorViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            AuthorToDelete = MainWindowViewModel.SelectedAuthor;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteAuthorCommand = new DelegateCommand(DeleteAuthor);
        }

        private void DeleteAuthor(object obj)
        {
            using var db = new BookstoreContext();

            db.Remove(AuthorToDelete);

            db.SaveChanges();
            MainWindowViewModel.RefreshAuthors();
        }
    }
}
