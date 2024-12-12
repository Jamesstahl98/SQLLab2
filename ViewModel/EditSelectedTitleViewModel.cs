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

        public EditSelectedTitleViewModel(MainWindowViewModel mainWindowViewModel, bool newTitle)
        {
            MainWindowViewModel = mainWindowViewModel;

            AllAuthors = MainWindowViewModel.Authors;
            if (!newTitle)
            {
                SelectedBook = new Book(mainWindowViewModel.SelectedBook);

                EditableAuthors = new ObservableCollection<EditableAuthor>(
                SelectedBook.Authors.Select(author =>
                    new EditableAuthor
                    {
                        SelectedAuthor = AllAuthors.FirstOrDefault(a => a.Id == author.Id) ?? AllAuthors.FirstOrDefault()
                    })
            );
            }

            else
            {
                SelectedBook = new Book();

                EditableAuthors = new ObservableCollection<EditableAuthor>();
            }

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
            bool isNewBook = false;

            using var db = new BookstoreContext();

            var originalBook = db.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .FirstOrDefault(b => b.Isbn == SelectedBook.Isbn);

            if (originalBook == null)
            {
                originalBook = new Book();
                isNewBook = true;
            }

            SaveChangesToBook(originalBook);

            if(isNewBook)
            {
                db.Books.Add(originalBook);
            }

            if (SelectedBook.Publisher != null)
            {
                var trackedPublisher = db.Publishers.FirstOrDefault(p => p.Id == SelectedBook.Publisher.Id);
                if (trackedPublisher != null)
                {
                    originalBook.Publisher = trackedPublisher;
                }
                else
                {
                    throw new InvalidOperationException($"Publisher with ID {SelectedBook.Publisher.Id} not found in the database.");
                }
            }

            if (originalBook.Authors != null)
            {
                originalBook.Authors.Clear();
            }
            else
            {
                originalBook.Authors = new ObservableCollection<Author>();
            }
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
            if (isNewBook)
            {
                MainWindowViewModel.AddBookToStoreSupplies(originalBook);
            }
            MainWindowViewModel.RefreshBooks();
        }

        private void SaveChangesToBook(Book book)
        {
            book.Title = SelectedBook.Title;
            book.Isbn = SelectedBook.Isbn;
            book.Language = SelectedBook.Language;
            book.PublishDate = SelectedBook.PublishDate;
            book.Price = SelectedBook.Price;
            book.Pages = SelectedBook.Pages;
        }
    }
}
