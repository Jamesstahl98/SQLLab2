using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class EditSelectedGenreViewModel : ViewModelBase
    {
        public MainWindowViewModel MainWindowViewModel {  get; set; }
        public string PendingGenreName { get; set; }
        public int? GenreId { get; set; }
        public DelegateCommand UpdateGenreAsyncCommand { get; private set; }

        public EditSelectedGenreViewModel(MainWindowViewModel mainWindowViewModel, bool isNewGenre)
        {
            MainWindowViewModel = mainWindowViewModel;
            if(!isNewGenre)
            {
                GenreId = mainWindowViewModel.SelectedGenre.Id;
                PendingGenreName = mainWindowViewModel.SelectedGenre.GenreName;
            }

            InitializeCommands();
        }
        private void InitializeCommands()
        {
            UpdateGenreAsyncCommand = new DelegateCommand(async obj => await (UpdateGenreAsync(obj)));
        }

        private async Task UpdateGenreAsync(object obj)
        {
            using var db = new BookstoreContext();
            if (GenreId != null)
            {
                var genreToEdit = await db.Genres.Where(g => g.Id == GenreId).FirstOrDefaultAsync();
                genreToEdit.GenreName = PendingGenreName;

                var genreViewModel = MainWindowViewModel.Genres.FirstOrDefault(g => g.Id == GenreId);
                if (genreViewModel != null)
                {
                    genreViewModel.GenreName = PendingGenreName;
                }

                foreach (var book in MainWindowViewModel.Books)
                {
                    var referencedGenre = book.Genres.FirstOrDefault(g => g.Id == GenreId);
                    if (referencedGenre != null)
                    {
                        referencedGenre.GenreName = PendingGenreName;
                    }
                }
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            else
            {
                try
                {
                    var newGenre = new Genre() { GenreName = PendingGenreName };
                    await db.Genres.AddAsync(newGenre);
                    await db.SaveChangesAsync();
                    MainWindowViewModel.Genres.Add(new GenreViewModel(newGenre));
                }
                catch (DbUpdateException ex)
                {
                    MainWindowViewModel.ShowMessage?.Invoke($"Database Update Error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
    }
}
