using PowerLogReader.Modules.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace PowerLogReader.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ICommand ClosingCommand { get; }

        public MainWindowViewModel(IPowerLogService powerLogService)
        {
            ClosingCommand = new DelegateCommand(() =>
            {
                powerLogService.AbortScan();
            });
        }
    }
}
