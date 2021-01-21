using PowerLogReader.Events;
using PowerLogReader.Modules.Services;
using PowerLogReader.Modules.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PowerLogReader.Modules.ViewModels
{
    public class MenuControlViewModel : BindableBase
    {
        public ICommand ExitCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AboutCommand { get; }

        private IPowerLogService PowerLogService;

        public MenuControlViewModel(IDialogService dialogService, IEventAggregator eventAggregator, IPowerLogService powerLogService)
        {
            PowerLogService = powerLogService;
            ExitCommand = new DelegateCommand(() =>
            {
                Application.Current.Shutdown();
            });
            SettingsCommand = new DelegateCommand(() =>
            {
                dialogService.ShowDialog(nameof(SettingsDialog), (x) =>
                {
                    if (x.Result == ButtonResult.Retry)
                    {
                        Task.Run(() => PowerLogService.ScanEventLogsAsync());
                    }
                    else
                    {
                        eventAggregator.GetEvent<DateChangedEvent>().Publish(PowerLogService.LastSelectedDate);
                    }
                });
            });
            AboutCommand = new DelegateCommand(() =>
            {
                dialogService.ShowDialog(nameof(AboutBoxControl), (x) => { });
            });
        }
    }
}
