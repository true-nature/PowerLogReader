using PowerLogReader.Modules.ViewModels;
using Reactive.Bindings;
using System;
using System.Windows.Controls;

namespace PowerLogReader.Modules.Views
{
    /// <summary>
    /// Interaction logic for CalendarControl.xaml
    /// </summary>
    public partial class CalendarControl : UserControl
    {
        public CalendarControl()
        {
            InitializeComponent();
            Loaded += CalendarControl_Loaded;
        }

        private ReadOnlyReactiveCollection<CalendarDateRange> BlackoutDates { get; set; }

        private void CalendarControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = DataContext as CalendarControlViewModel;
            BlackoutDates = viewModel.BlackoutDates;
            viewModel.ScanCompleted.Subscribe(OnScanCompleted);
        }

        private void OnScanCompleted(bool completed)
        {
            if (completed)
            {
                foreach (var r in BlackoutDates)
                {
                    this.Calendar.BlackoutDates.Add(r);
                }
            }
            else
            {
                this.Calendar.BlackoutDates.Clear();
            }
        }
    }
}
