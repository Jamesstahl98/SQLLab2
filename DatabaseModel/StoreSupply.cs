using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class StoreSupply
{
    public int StoreId { get; set; }

    public string Isbn { get; set; } = null!;

    public int Amount { get; set; }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
