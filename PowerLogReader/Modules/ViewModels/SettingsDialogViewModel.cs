﻿using PowerLogReader.Core;
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

        public ReactivePropertySlim<int> RoundingRuleIndex { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> FirstDayIndex { get; } = new ReactivePropertySlim<int>();

        public ReactiveProperty<int> DayOffset { get; } = new ReactiveProperty<int>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveProperty<int> MaxDays { get; } = new ReactiveProperty<int>();

        public ReactivePropertySlim<bool> EnableBlackoutDates { get; } = new ReactivePropertySlim<bool>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public SettingsDialogViewModel(IPreferenceService preference)
        {
            Preference = preference;
            RoundingRuleIndex.Value = (int)Preference.Rounding;
            RoundingRuleIndex.Subscribe(OnRoundingRuleChanged);

            FirstDayIndex.Value = (int)Preference.FirstDayOfWeek;
            FirstDayIndex.Subscribe(OnFirstDayOfWeekChanged);

            DayOffset.Value = (int)Preference.DayOffset.TotalMinutes;
            DayOffset.Subscribe(OnDayOffsetChanged);

            MaxDays.Value = Preference.MaxDays;
            MaxDays.Subscribe(OnMaxDaysChanged);

            EnableBlackoutDates.Value = Preference.EnableBlackoutDates;
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
            ButtonResult result = RescanRequired ? ButtonResult.Retry : ButtonResult.OK;
            RequestClose?.Invoke(new DialogResult(result));
        }

        private void OnRoundingRuleChanged(int value)
        {
            Preference.Rounding = (RoundingRule)Enum.ToObject(typeof(RoundingRule), value);
        }

        private void OnFirstDayOfWeekChanged(int value)
        {
            Preference.FirstDayOfWeek = (DayOfWeek)Enum.ToObject(typeof(DayOfWeek), value);
        }

        private void OnDayOffsetChanged(int value)
        {
            Preference.DayOffset = TimeSpan.FromMinutes(value);
            RescanRequired = true;
        }

        private void OnMaxDaysChanged(int value)
        {
            Preference.MaxDays = value;
            RescanRequired = true;
        }

        private void OnEnableBlackoutDatesChanged(bool value)
        {
            Preference.EnableBlackoutDates = value;
            RescanRequired = true;
        }
    }
}
