using PowerLogReader.Modules.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using System;
using System.Windows.Input;

namespace PowerLogReader.Modules.ViewModels
{
    public class SettingsDialogViewModel : BindableBase, IDialogAware
    {
        private bool RescanRequired = false;

        public ICommand OkCommand { get; }
        public IPreferenceService Preference { get; }

        public ReactivePropertySlim<double> DayOffset { get; } = new ReactivePropertySlim<double>(mode:ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<long> MaxDays { get; } = new ReactiveProperty<long>();

        public SettingsDialogViewModel(IPreferenceService preference)
        {
            Preference = preference;
            DayOffset.Value = Preference.DayOffset.TotalHours;
            DayOffset.Subscribe((x) =>
            {
                Preference.DayOffset = TimeSpan.FromHours(x);
                RescanRequired = true;
            });
            MaxDays.Value = Preference.MaxDays;
            MaxDays.Subscribe((x) =>
            {
                Preference.MaxDays = x;
                RescanRequired = true;
            });
            OkCommand = new DelegateCommand(() => {
                ButtonResult result = RescanRequired ? ButtonResult.Retry :  ButtonResult.OK;
                RequestClose?.Invoke(new DialogResult(result));
            });
        }

        public string Title => "Settings";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            RescanRequired = false;
        }
    }
}
