using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SQLLab2;

public partial class StoreSupply : INotifyPropertyChanged
{
    private int _amount;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int StoreId { get; set; }

    public string Isbn { get; set; } = null!;

    public int Amount
    {
        get => _amount;
        set
        {
            if (_amount != value)
            {
                _amount = value;
                OnPropertyChanged();
            }
        }
    }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
