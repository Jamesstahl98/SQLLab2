using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    internal class StoreSupplyViewModel : ViewModelBase
    {
        private int _amount;

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
                    RaisePropertyChanged();
                }
            }
        }
        public string Title { get; set; } = null!;
        public string Authors { get; set; } = null!;

    }
}
