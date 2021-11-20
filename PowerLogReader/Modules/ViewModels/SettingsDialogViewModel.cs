using PowerLogReader.Core;
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
        public IPreferenceService PreferenceService { get; }

        public ReactivePropertySlim<int> RoundingRuleIndex { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> RoundUnit { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> StartMargin { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> EndMargin { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> FirstDayIndex { get; } = new ReactivePropertySlim<int>();

        public ReactiveProperty<int> DayOffset { get; } = new ReactiveProperty<int>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveProperty<int> MaxDays { get; } = new ReactiveProperty<int>();

        public ReactivePropertySlim<bool> EnableBlackoutDates { get; } = new ReactivePropertySlim<bool>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public SettingsDialogViewModel(IPreferenceService preferenceService)
        {
            PreferenceService = preferenceService;
            var preference = PreferenceService.Preference;
            RoundingRuleIndex.Value = (int)preference.Rounding;
            RoundUnit.Value = preference.RoundUnit;
            StartMargin.Value = preference.StartMargin;
            EndMargin.Value = preference.EndMargin;

            FirstDayIndex.Value = (int)preference.FirstDayOfWeek;

            DayOffset.Value = (int)preference.DayOffsetMinutes;
            DayOffset.Subscribe(OnDayOffsetChanged);

            MaxDays.Value = preference.MaxDays;
            MaxDays.Subscribe(OnMaxDaysChanged);

            EnableBlackoutDates.Value = preference.EnableBlackoutDates;
            EnableBlackoutDates.Subscribe(OnEnableBlackoutDatesChanged);

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
            var preference = PreferenceService.Preference;
            preference.Rounding = (RoundingRule)Enum.ToObject(typeof(RoundingRule), RoundingRuleIndex.Value);
            preference.RoundUnit = RoundUnit.Value;
            preference.StartMargin = StartMargin.Value;
            preference.EndMargin = EndMargin.Value;
            preference.FirstDayOfWeek = (DayOfWeek)Enum.ToObject(typeof(DayOfWeek), FirstDayIndex.Value);
            preference.DayOffsetMinutes = DayOffset.Value;
            preference.MaxDays = MaxDays.Value;
            preference.EnableBlackoutDates = EnableBlackoutDates.Value;
            PreferenceService.Save();

            ButtonResult result = RescanRequired ? ButtonResult.Retry : ButtonResult.OK;
            RequestClose?.Invoke(new DialogResult(result));
        }

        private void OnDayOffsetChanged(int value)
        {
            RescanRequired = true;
        }

        private void OnMaxDaysChanged(int value)
        {
            RescanRequired = true;
        }

        private void OnEnableBlackoutDatesChanged(bool value)
        {
            RescanRequired = true;
        }
    }
}
