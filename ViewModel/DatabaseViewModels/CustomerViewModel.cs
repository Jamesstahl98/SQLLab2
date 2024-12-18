using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel;

internal class CustomerViewModel : ViewModelBase
{
    private readonly Customer _customer;

    public Customer Customer => _customer;

    public int Id
    {
        get => _customer.Id;
        set
        {
            if (_customer.Id != value)
            {
                _customer.Id = value;
                RaisePropertyChanged();
            }
        }
    }

    public string FirstName
    {
        get => _customer.FirstName;
        set
        {
            if (_customer.FirstName != value)
            {
                _customer.FirstName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }
    }

    public string LastName
    {
        get => _customer.LastName;
        set
        {
            if (_customer.LastName != value)
            {
                _customer.LastName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }
    }

    public DateOnly Birthdate
    {
        get => _customer.Birthdate;
        set
        {
            if (_customer.Birthdate != value)
            {
                _customer.Birthdate = value;
                RaisePropertyChanged();
            }
        }
    }

    public string City
    {
        get => _customer.City;
        set
        {
            if (_customer.City != value)
            {
                _customer.City = value;
                RaisePropertyChanged();
            }
        }
    }

    public string Address
    {
        get => _customer.Address;
        set
        {
            if (_customer.Address != value)
            {
                _customer.Address = value;
                RaisePropertyChanged();
            }
        }
    }

    public string PostalCode
    {
        get => _customer.PostalCode;
        set
        {
            if (_customer.PostalCode != value)
            {
                _customer.PostalCode = value;
                RaisePropertyChanged();
            }
        }
    }

    public string Email
    {
        get => _customer.Email;
        set
        {
            if (_customer.Email != value)
            {
                _customer.Email = value;
                RaisePropertyChanged();
            }
        }
    }

    public string FullName => _customer.FullName;

    public ObservableCollection<OrderViewModel> Orders { get;
        set; }

    public CustomerViewModel(Customer customer)
    {
        _customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Orders = new ObservableCollection<OrderViewModel>(
            _customer.Orders.Select(order => new OrderViewModel(order)));
    }
}
