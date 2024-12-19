using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel;

internal class AuthorViewModel : ViewModelBase
{
    private readonly Author _author;
    public Author Author
    {
        get => _author;
    }

    public int Id => _author.Id;

    public string FirstName
    {
        get => _author.FirstName;
        set
        {
            if (_author.FirstName != value)
            {
                _author.FirstName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }
    }

    public string LastName
    {
        get => _author.LastName;
        set
        {
            if (_author.LastName != value)
            {
                _author.LastName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }
    }

    public DateOnly BirthDate
    {
        get => _author.BirthDate;
        set
        {
            if (_author.BirthDate != value)
            {
                _author.BirthDate = value;
                RaisePropertyChanged();
            }
        }
    }

    public DateOnly? DeathDate
    {
        get => _author.DeathDate;
        set
        {
            if (_author.DeathDate != value)
            {
                _author.DeathDate = value;
                RaisePropertyChanged();
            }
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    public ObservableCollection<Book> BookIsbns { get; set; }

    public AuthorViewModel(Author author)
    {
        _author = author ?? throw new ArgumentNullException(nameof(author));
        BookIsbns = new ObservableCollection<Book>(_author.BookIsbns);
    }
}
