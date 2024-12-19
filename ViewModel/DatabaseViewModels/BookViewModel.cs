using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    internal class BookViewModel : ViewModelBase
    {
        private readonly Book _book;
        public Book Book
        {
            get => _book;
        }

        public BookViewModel(Book book)
        {
            _book = book ?? throw new ArgumentNullException(nameof(book));
        }

        private string _isbn;
        public string Isbn
        {
            get => _isbn ?? _book.Isbn;
            set
            {
                if (_isbn != value)
                {
                    _isbn = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _title;
        public string Title
        {
            get => _title ?? _book.Title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _language;
        public string Language
        {
            get => _language ?? _book.Language;
            set
            {
                if (_language != value)
                {
                    _language = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _price;
        public int Price
        {
            get => _price != 0 ? _price : _book.Price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _pages;

        public int Pages
        {
            get => _pages != 0 ? _pages : _book.Pages;
            set
            {
                if (_pages != value)
                {
                    _pages = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateOnly? _publishDate;
        public DateOnly? PublishDate
        {
            get => _publishDate != null ? _publishDate : _book.PublishDate;
            set 
            {
                if (_publishDate != value)
                {
                    _publishDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Publisher? _publisher;
        public Publisher? Publisher
        {
            get => _publisher ?? _book.Publisher;
            set
            {
                    _publisher = value;
                    RaisePropertyChanged();
            }
        }

        public ObservableCollection<Author> Authors => _book.Authors;

        public ObservableCollection<Genre> Genres => _book.Genres;
    }
}
