using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;

namespace SQLLab2;

public partial class Author
{
    public string FullName => $"{FirstName} {LastName}";

    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public DateOnly? DeathDate { get; set; }

    public virtual ICollection<Book> BookIsbns { get; set; } = new List<Book>();

    public Author()
    {

    }
    public Author(Author other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        Id = other.Id;
        FirstName = other.FirstName;
        LastName = other.LastName;
        BirthDate = other.BirthDate;
        DeathDate = other.DeathDate;
        BookIsbns = new List<Book>(other.BookIsbns);
    }
}
