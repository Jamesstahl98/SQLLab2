using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel;

internal class OrderViewModel : ViewModelBase
{
    private readonly Order _order;
    private CustomerViewModel _customer;

    public Order Order => _order;

    public int Id
    {
        get => _order.Id;
        set
        {
            if (_order.Id != value)
            {
                _order.Id = value;
                RaisePropertyChanged();
            }
        }
    }

    public int CustomerId
    {
        get => _order.CustomerId;
        set
        {
            if (_order.CustomerId != value)
            {
                _order.CustomerId = value;
                RaisePropertyChanged();
            }
        }
    }

    public DateTime DateAndTimePlaced
    {
        get => _order.DateAndTimePlaced;
        set
        {
            if (_order.DateAndTimePlaced != value)
            {
                _order.DateAndTimePlaced = value;
                RaisePropertyChanged();
            }
        }
    }

    public string City
    {
        get => _order.City;
        set
        {
            if (_order.City != value)
            {
                _order.City = value;
                RaisePropertyChanged();
            }
        }
    }

    public string Address
    {
        get => _order.Address;
        set
        {
            if (_order.Address != value)
            {
                _order.Address = value;
                RaisePropertyChanged();
            }
        }
    }

    public string PostalCode
    {
        get => _order.PostalCode;
        set
        {
            if (_order.PostalCode != value)
            {
                _order.PostalCode = value;
                RaisePropertyChanged();
            }
        }
    }

    public CustomerViewModel Customer
    {
        get => _customer;
        set
        {
            if (_customer != value)
            {
                _customer = value;
                _order.Customer = _customer?.Customer;
                RaisePropertyChanged();
            }
        }
    }

    public ObservableCollection<OrderBookJt> OrderBookJts { get; set; }

    public OrderViewModel(Order order, CustomerViewModel customerViewModel = null)
    {
        _order = order ?? throw new ArgumentNullException(nameof(order));
        OrderBookJts = new ObservableCollection<OrderBookJt>(_order.OrderBookJts);

        if (customerViewModel != null)
        {
            Customer = customerViewModel;
        }
        else if (order.Customer != null)
        {
            Customer = new CustomerViewModel(order.Customer, true);
        }
    }
}
