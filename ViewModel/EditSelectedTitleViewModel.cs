using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public bool ErrorThrown { get; set; } = false;
        public string ErrorMessage { get; set; }

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

        public DelegateCommand UpdateTitleAsyncCommand { get; private set; }
        public DelegateCommand AddAuthorCommand { get; private set; }
        public DelegateCommand RemoveAuthorCommand { get; private set; }

        public EditSelectedTitleViewModel(MainWindowViewModel mainWindowViewModel, bool newTitle)
        {
            MainWindowViewModel = mainWindowViewModel;

            AllAuthors = MainWindowViewModel.Authors;
            if (!newTitle && mainWindowViewModel.SelectedBook != null)
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
            UpdateTitleAsyncCommand = new DelegateCommand(async obj => await UpdateTitleAsync(obj));
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

        private async Task UpdateTitleAsync(object obj)
        {
            bool isNewBook = false;

            using var db = new BookstoreContext();

            var originalBook = await db.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Isbn == SelectedBook.Isbn);

            if (originalBook == null)
            {
                originalBook = new Book();
                isNewBook = true;
            }

            SaveChangesToBook(originalBook);

            if(isNewBook)
            {
                try
                {
                    await db.Books.AddAsync(originalBook);
                }
                catch (Exception ex)
                {
                    MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
                    return;
                }
            }

            if (SelectedBook.Publisher != null)
            {
                var trackedPublisher = await db.Publishers.FirstOrDefaultAsync(p => p.Id == SelectedBook.Publisher.Id);
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
                var trackedAuthor = await db.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);

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

            try
            {
                await db.SaveChangesAsync();

                if (isNewBook)
                {
                    await MainWindowViewModel.AddBookToStoreSuppliesAsync(originalBook);
                }
                await MainWindowViewModel.RefreshBooksAsync();
            }
            catch (Exception ex)
            {
                MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
            }
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
