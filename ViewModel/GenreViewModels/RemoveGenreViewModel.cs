﻿using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class RemoveGenreViewModel
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public GenreViewModel GenreViewModelToDelete { get; set; }
        public DelegateCommand DeleteGenreAsyncCommand { get; private set; }

        public RemoveGenreViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            GenreViewModelToDelete = MainWindowViewModel.SelectedGenre;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            DeleteGenreAsyncCommand = new DelegateCommand(async obj => await DeleteGenreAsync(obj));
        }

        private async Task DeleteGenreAsync(object obj)
        {
            using var db = new BookstoreContext();

            db.Remove(GenreViewModelToDelete.Genre);
            MainWindowViewModel.Genres.Remove(GenreViewModelToDelete);

            await db.SaveChangesAsync();
        }
    }
}
