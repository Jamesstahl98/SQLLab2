using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class OrderBookJt
{
    public int OrderId { get; set; }

    public string BookIsbn { get; set; } = null!;

    public int UnitPrice { get; set; }

    public int Amount { get; set; }

    public virtual Book BookIsbnNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
