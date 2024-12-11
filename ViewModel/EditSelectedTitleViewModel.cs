using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    internal class EditSelectedTitleViewModel : ViewModelBase
    {
        public class EditableAuthor : ViewModelBase
        {
            private Author _selectedAuthor;

            public Author SelectedAuthor
            {
                get => _selectedAuthor;
                set
                {
                    _selectedAuthor = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Book _selectedBook;
        private ObservableCollection<EditableAuthor> _editableAuthors;

        public ObservableCollection<EditableAuthor> EditableAuthors
        {
            get => _editableAuthors;
            set
            {
                _editableAuthors = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Author> AllAuthors { get; set; }
        public MainWindowViewModel MainWindowViewModel { get; set; }

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand UpdateTitleCommand { get; private set; }
        public DelegateCommand AddAuthorCommand { get; private set; }
        public DelegateCommand RemoveAuthorCommand { get; private set; }

        public EditSelectedTitleViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;

            AllAuthors = MainWindowViewModel.Authors;

            SelectedBook = new Book(mainWindowViewModel.SelectedBook);


            EditableAuthors = new ObservableCollection<EditableAuthor>(
                SelectedBook.Authors.Select(author =>
                    new EditableAuthor
                    {
                        SelectedAuthor = AllAuthors.FirstOrDefault(a => a.Id == author.Id) ?? AllAuthors.FirstOrDefault()
                    })
            );

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateTitleCommand = new DelegateCommand(UpdateTitle);
            AddAuthorCommand = new DelegateCommand(AddAuthor);
            RemoveAuthorCommand = new DelegateCommand(RemoveAuthor);
        }

        private void RemoveAuthor(object obj)
        {
            EditableAuthors.Remove((EditableAuthor)obj);
        }

        private void AddAuthor(object obj)
        {
            EditableAuthors.Add(new EditableAuthor());
        }

        private void UpdateTitle(object obj)
        {
            using var db = new BookstoreContext();

            var originalBook = db.Books
                .Include(b => b.Authors)
                .FirstOrDefault(b => b.Isbn == SelectedBook.Isbn);

            if (originalBook == null) return;

            originalBook.Title = SelectedBook.Title;
            originalBook.Isbn = SelectedBook.Isbn;
            originalBook.Language = SelectedBook.Language;
            originalBook.PublishDate = SelectedBook.PublishDate;
            originalBook.Price = SelectedBook.Price;

            originalBook.Authors.Clear();
            foreach (var editableAuthor in EditableAuthors)
            {
                var author = editableAuthor.SelectedAuthor;
                var trackedAuthor = db.Authors.FirstOrDefault(a => a.Id == author.Id);

                if (trackedAuthor != null)
                {
                    originalBook.Authors.Add(trackedAuthor);
                }
                else
                {
                    db.Authors.Attach(author);
                    originalBook.Authors.Add(author);
                }
            }
            db.SaveChanges();
            MainWindowViewModel.RefreshBooks();
        }
    }
}
