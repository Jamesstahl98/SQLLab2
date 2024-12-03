using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class OrdersPerGenre
{
    public string GenreName { get; set; } = null!;

    public int? Orders { get; set; }

    public int? IncomeFromGenre { get; set; }
}
