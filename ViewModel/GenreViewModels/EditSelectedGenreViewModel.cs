using Microsoft.EntityFrameworkCore;
using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class EditSelectedGenreViewModel
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
                var genreToEdit = await db.Genres
                    .Where(g => g.Id == GenreId)
                    .FirstOrDefaultAsync();

                genreToEdit.GenreName = PendingGenreName;
            }
            else
            {
                await db.Genres.AddAsync(new Genre() { GenreName = PendingGenreName });
            }
            await db.SaveChangesAsync();

            await MainWindowViewModel.RefreshGenresAsync();
        }
    }
}
