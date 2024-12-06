using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    internal class EditSelectedTitleViewModel : ViewModelBase
    {
        private Book _selectedBook;

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

        public EditSelectedTitleViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            SelectedBook = MainWindowViewModel.SelectedBook;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateTitleCommand = new DelegateCommand(UpdateTitle);
        }

        private void UpdateTitle(object obj)
        {
            MainWindowViewModel.SelectedBook = SelectedBook;
        }
    }
}
