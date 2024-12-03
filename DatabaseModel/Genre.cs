using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Genre
{
    public int Id { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<Book> BookIsbns { get; set; } = new List<Book>();
}
