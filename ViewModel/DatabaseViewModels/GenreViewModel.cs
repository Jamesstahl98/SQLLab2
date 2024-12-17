using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel;

internal class GenreViewModel : ViewModelBase
{
    private readonly Genre _genre;
    public Genre Genre
    {
        get => _genre;
    }

    public GenreViewModel(Genre genre)
    {
        _genre = genre ?? throw new ArgumentNullException(nameof(genre));
    }

    public int Id
    {
        get => _genre.Id;
        set
        {
            if (_genre.Id != value)
            {
                _genre.Id = value;
                RaisePropertyChanged();
            }
        }
    }

    public string GenreName
    {
        get => _genre.GenreName;
        set
        {
            if (_genre.GenreName != value)
            {
                _genre.GenreName = value;
                RaisePropertyChanged();
            }
        }
    }

    public ObservableCollection<BookViewModel> Books
    {
        get => new ObservableCollection<BookViewModel>(
            _genre.BookIsbns.Select(b => new BookViewModel(b)));
    }
}
