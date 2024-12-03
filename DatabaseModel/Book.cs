using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Book
{
    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Language { get; set; } = null!;

    public int Price { get; set; }

    public int Pages { get; set; }

    public DateTime? PublishDate { get; set; }

    public int? PublisherId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<StoreSupply> StoreSupplies { get; set; } = new List<StoreSupply>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
