using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime DateAndTimePlaced { get; set; }

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderBookJt> OrderBookJts { get; set; } = new List<OrderBookJt>();

    public Order()
    {

    }

    public Order(Order other)
    {
        Id = other.Id;
        CustomerId = other.CustomerId;
        DateAndTimePlaced = other.DateAndTimePlaced;
        City = other.City;
        Address = other.Address;
        PostalCode = other.PostalCode;
        Customer = other.Customer;
    }
}
