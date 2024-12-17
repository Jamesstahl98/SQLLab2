using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace SQLLab2;

public partial class Book
{
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

    public virtual ObservableCollection<Author> Authors { get; set; }

    public virtual ObservableCollection<Genre> Genres { get; set; }
}
