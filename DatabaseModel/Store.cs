using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Store
{
    public int Id { get; set; }

    public string StoreName { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public virtual ICollection<StoreSupply> StoreSupplies { get; set; } = new List<StoreSupply>();
}
