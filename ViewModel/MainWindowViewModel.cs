using SQLLab2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLab2.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public DelegateCommand CreateNewDialogCommand { get; private set; }
        public event Action<string> CreateDialogRequested;
        public MainWindowViewModel()
        {
            InitializeCommands();
        }
        private void InitializeCommands()
        {
            CreateNewDialogCommand = new DelegateCommand(CreateNewDialog);
        }

        private void CreateNewDialog(object obj)
        {
            string className = obj as string;
            CreateDialogRequested?.Invoke(className);
        }
    }
}
