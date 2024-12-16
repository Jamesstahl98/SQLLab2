using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Customer
{
    public string FullName => $"{FirstName} {LastName}";

    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public Customer()
    {

    }
    public Customer(Customer other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        Id = other.Id;
        FirstName = other.FirstName;
        LastName = other.LastName;
        Birthdate = other.Birthdate;
        City = other.City;
        Address = other.Address;
        PostalCode = other.PostalCode;
        Email = other.Email;
    }
}
