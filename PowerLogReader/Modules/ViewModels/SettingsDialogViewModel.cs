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

        public ReactiveProperty<int> DayOffset { get; } = new ReactiveProperty<int>(mode:ReactivePropertyMode.DistinctUntilChanged);
        public ICommand DayOffsetUpCommand { get; }
        public ICommand DayOffsetDownCommand { get; }

        public ReactiveProperty<int> MaxDays { get; } = new ReactiveProperty<int>();
        public ICommand MaxDaysUpCommand { get; }
        public ICommand MaxDaysDownCommand { get; }

        public SettingsDialogViewModel(IPreferenceService preference)
        {
            Preference = preference;
            DayOffset.Value = (int)Preference.DayOffset.TotalMinutes;
            DayOffset.Subscribe(OnDayOffsetChanged);
            DayOffsetUpCommand = new DelegateCommand(OnDayOffsetUp);
            DayOffsetDownCommand = new DelegateCommand(OnDayOffsetDown);

            MaxDays.Value = Preference.MaxDays;
            MaxDays.Subscribe(OnMaxDaysChanged);
            MaxDaysUpCommand = new DelegateCommand(OnMaxDaysUp);
            MaxDaysDownCommand = new DelegateCommand(OnMaxDaysDown);

            OkCommand = new DelegateCommand(OnOkCommand);
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

        private void OnOkCommand()
        {
            ButtonResult result = RescanRequired ? ButtonResult.Retry : ButtonResult.OK;
            RequestClose?.Invoke(new DialogResult(result));
        }

        private void OnDayOffsetChanged(int value)
        {
            Preference.DayOffset = TimeSpan.FromMinutes(value);
            RescanRequired = true;
        }

        private void OnDayOffsetUp()
        {
            if (DayOffset.Value < 720)
            {
                DayOffset.Value++;
            }
        }

        private void OnDayOffsetDown()
        {
            if (DayOffset.Value > -720)
            {
                DayOffset.Value--;
            }
        }

        private void OnMaxDaysChanged(int value)
        {
            Preference.MaxDays = value;
            RescanRequired = true;
        }

        private void OnMaxDaysUp()
        {
            if (MaxDays.Value < 366)
            {
                MaxDays.Value++;
            }
        }

        private void OnMaxDaysDown()
        {
            if (MaxDays.Value > 2)
            {
                MaxDays.Value--;
            }
        }
    }
}
