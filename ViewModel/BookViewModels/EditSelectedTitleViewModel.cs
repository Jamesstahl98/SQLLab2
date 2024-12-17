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
            private AuthorViewModel _selectedAuthor;

            public AuthorViewModel SelectedAuthor
            {
                get => _selectedAuthor;
                set
                {
                    _selectedAuthor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public class EditableGenre : ViewModelBase
        {
            private GenreViewModel _selectedGenre;

            public GenreViewModel SelectedGenre
            {
                get => _selectedGenre;
                set
                {
                    _selectedGenre = value;
                    RaisePropertyChanged();
                }
            }
        }

        private BookViewModel _selectedBook;
        private ObservableCollection<EditableAuthor> _editableAuthors;
        private ObservableCollection<EditableGenre> _editableGenres;

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
        public ObservableCollection<EditableGenre> EditableGenres
        {
            get => _editableGenres;
            set
            {
                _editableGenres = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<AuthorViewModel> AllAuthors { get; set; }
        public ObservableCollection<GenreViewModel> AllGenres { get; set; }
        public MainWindowViewModel MainWindowViewModel { get; set; }

        public BookViewModel SelectedBook
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
        public DelegateCommand AddGenreCommand { get; private set; }
        public DelegateCommand RemoveGenreCommand { get; private set; }

        public EditSelectedTitleViewModel(MainWindowViewModel mainWindowViewModel, bool newTitle)
        {
            MainWindowViewModel = mainWindowViewModel;

            AllAuthors = MainWindowViewModel.Authors;
            AllGenres = MainWindowViewModel.Genres;

            if (!newTitle && mainWindowViewModel.SelectedBook != null)
            {
                SelectedBook = mainWindowViewModel.SelectedBook;

                EditableAuthors = new ObservableCollection<EditableAuthor>(
                SelectedBook.Authors.Select(author =>
                    new EditableAuthor
                    {
                        SelectedAuthor = AllAuthors.FirstOrDefault(a => a.Id == author.Id) ?? AllAuthors.FirstOrDefault()
                    })
                );

                EditableGenres = new ObservableCollection<EditableGenre>(
                SelectedBook.Genres.Select(genre =>
                    new EditableGenre
                    {
                        SelectedGenre = AllGenres.FirstOrDefault(g => g.Id == genre.Id) ?? AllGenres.FirstOrDefault()
                    })
                );
            }

            else
            {
                //Test
                SelectedBook = new BookViewModel(new Book());

                EditableAuthors = new ObservableCollection<EditableAuthor>();
                EditableGenres = new ObservableCollection<EditableGenre>();
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateTitleAsyncCommand = new DelegateCommand(async obj => await UpdateTitleAsync(obj));
            AddAuthorCommand = new DelegateCommand(AddAuthor);
            RemoveAuthorCommand = new DelegateCommand(RemoveAuthor);
            AddGenreCommand = new DelegateCommand(AddGenre);
            RemoveGenreCommand = new DelegateCommand(RemoveGenre);
        }

        private void RemoveAuthor(object obj)
        {
            EditableAuthors.Remove((EditableAuthor)obj);
        }

        private void AddAuthor(object obj)
        {
            EditableAuthors.Add(new EditableAuthor());
        }

        private void RemoveGenre(object obj)
        {
            EditableGenres.Remove((EditableGenre)obj);
        }

        private void AddGenre(object obj)
        {
            EditableGenres.Add(new EditableGenre());
        }

        private async Task UpdateTitleAsync(object obj)
        {
            bool isNewBook = false;

            using var db = new BookstoreContext();

            var originalBook = await db.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .Include(b => b.Genres)
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

            await AddAuthorsToBookAsync(originalBook, db);
            await AddGenresToBookAsync(originalBook, db);
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

        private async Task AddAuthorsToBookAsync(Book book, BookstoreContext db)
        {
            if (book.Authors != null)
            {
                book.Authors.Clear();
            }
            else
            {
                book.Authors = new ObservableCollection<Author>();
            }
            foreach (var editableAuthor in EditableAuthors)
            {
                var selectedAuthorViewModel = editableAuthor.SelectedAuthor;
                var trackedAuthor = await db.Authors.FirstOrDefaultAsync(a => a.Id == selectedAuthorViewModel.Id);

                if (trackedAuthor != null)
                {
                    book.Authors.Add(trackedAuthor);
                }
                else
                {
                    db.Authors.Attach(selectedAuthorViewModel.Author);
                    book.Authors.Add(selectedAuthorViewModel.Author);
                }
            }
        }
        private async Task AddGenresToBookAsync(Book book, BookstoreContext db)
        {
            if (book.Genres != null)
            {
                book.Genres.Clear();
            }
            else
            {
                book.Genres = new ObservableCollection<Genre>();
            }
            foreach (var editableGenre in EditableGenres)
            {
                var selectedGenreViewModel = editableGenre.SelectedGenre;
                var trackedGenre = await db.Genres.FirstOrDefaultAsync(g => g.Id == selectedGenreViewModel.Id);

                if (trackedGenre != null)
                {
                    book.Genres.Add(trackedGenre);
                }
                else
                {
                    db.Genres.Attach(selectedGenreViewModel.Genre);
                    book.Genres.Add(selectedGenreViewModel.Genre);
                }
            }
        }
    }
}
