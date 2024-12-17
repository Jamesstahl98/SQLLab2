using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    public class StoreSupplyViewModel : INotifyPropertyChanged
    {
        private readonly StoreSupply _storeSupply;

        public int StoreId => _storeSupply.StoreId;

        public string Isbn => _storeSupply.Isbn;

        public int Amount
        {
            get => _storeSupply.Amount;
            set
            {
                if (_storeSupply.Amount != value)
                {
                    _storeSupply.Amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public string BookTitle => _storeSupply.IsbnNavigation?.Title ?? "N/A";

        public string BookAuthors => _storeSupply.IsbnNavigation != null
            ? string.Join(", ", _storeSupply.IsbnNavigation.Authors.Select(a => a.FullName))
            : "N/A";

        public StoreSupplyViewModel(StoreSupply storeSupply)
        {
            _storeSupply = storeSupply ?? throw new ArgumentNullException(nameof(storeSupply));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
