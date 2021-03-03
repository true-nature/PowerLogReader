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
        private CalendarControlViewModel ViewModel => DataContext as CalendarControlViewModel;
        public CalendarControl()
        {
            InitializeComponent();
            Loaded += CalendarControl_Loaded;
        }

        private void CalendarControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.ScanCompleted.Subscribe(OnScanCompleted);
        }

        private void OnScanCompleted(bool completed)
        {
            if (completed)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var BlackoutDates = ViewModel.GetBlackoutDates();
                    foreach (var r in BlackoutDates)
                    {
                        this.Calendar.BlackoutDates.Add(new CalendarDateRange(r.Item1, r.Item2));
                    }
                });
            }
            else
            {
                this.Dispatcher.Invoke(() => this.Calendar.BlackoutDates.Clear());
            }
        }
    }
}
