using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace SQLLab2;

public partial class Book: INotifyPropertyChanged
{
    private ObservableCollection<Author> _authors;
    private ObservableCollection<Genre> _genres;

    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Language { get; set; } = null!;

    public int Price { get; set; }

    public int Pages { get; set; }

    public DateOnly? PublishDate { get; set; }

    public int? PublisherId { get; set; }

    public virtual ICollection<OrderBookJt> OrderBookJts { get; set; } = new List<OrderBookJt>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<StoreSupply> StoreSupplies { get; set; } = new List<StoreSupply>();

    public virtual ObservableCollection<Author> Authors
    {
        get => _authors;
        set
        {
            _authors = value;
            OnPropertyChanged();
        }
    }

    public virtual ObservableCollection<Genre> Genres 
    {
        get => _genres;
        set 
        { 
            _genres = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Book()
    {

    }
    public Book(Book other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        Isbn = other.Isbn;
        Title = other.Title;
        Language = other.Language;
        Price = other.Price;
        Pages = other.Pages;
        PublishDate = other.PublishDate;
        PublisherId = other.PublisherId;
        Publisher = other.Publisher;
        StoreSupplies = new List<StoreSupply>(other.StoreSupplies);
        Authors = new ObservableCollection<Author>(other.Authors);
        Genres = new ObservableCollection<Genre>(other.Genres);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
