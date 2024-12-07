using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    internal class EditSelectedTitleViewModel : ViewModelBase
    {
        private Book _selectedBook;
        private ObservableCollection<Author> _editableAuthors;
        public ObservableCollection<Author> EditableAuthors
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

        public EditSelectedTitleViewModel(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            SelectedBook = new Book(mainWindowViewModel.SelectedBook);
            EditableAuthors = new ObservableCollection<Author>(SelectedBook.Authors);
            AllAuthors = MainWindowViewModel.Authors;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            UpdateTitleCommand = new DelegateCommand(UpdateTitle);
        }

        private void UpdateTitle(object obj)
        {
            var originalBook = MainWindowViewModel.SelectedBook;

            originalBook.Title = SelectedBook.Title;
            originalBook.Isbn = SelectedBook.Isbn;
            originalBook.Language = SelectedBook.Language;
            originalBook.PublishDate = SelectedBook.PublishDate;
            originalBook.Price = SelectedBook.Price;

            originalBook.Authors = EditableAuthors;
        }
    }
}
