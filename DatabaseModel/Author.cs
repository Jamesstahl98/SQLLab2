using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public DateTime? DeathDate { get; set; }

    public virtual ICollection<Book> BookIsbns { get; set; } = new List<Book>();
}
