using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string BookIsbn { get; set; } = null!;

    public virtual Book BookIsbnNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
