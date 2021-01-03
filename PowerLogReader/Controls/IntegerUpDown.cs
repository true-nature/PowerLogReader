using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PowerLogReader.Controls
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PowerLogReader"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PowerLogReader;assembly=PowerLogReader"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:IntegerUpDown/>
    ///
    /// </summary>
    public class IntegerUpDown : Control
    {
        static IntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerUpDown), 
                new FrameworkPropertyMetadata(typeof(IntegerUpDown)));
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(0));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(Int32.MaxValue));

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(Int32.MinValue));

        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(nameof(Increment), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(1));

        public int Increment
        {
            get => (int)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(nameof(Delay), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(500));

        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(nameof(Interval), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(100));

        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        private void OnDownClicked(object sender, RoutedEventArgs a)
        {
            if (Value >= (MinValue + Increment))
            {
                Value -= Increment;
            }
        }

        private void OnUpClicked(object sender, RoutedEventArgs a)
        {
            if (Value <= (MaxValue - Increment))
            {
                Value += Increment;
            }
        }

        private RepeatButton UpButton;
        private RepeatButton DownButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.UpButton != null)
            {
                this.UpButton.Click -= OnUpClicked;
            }
            if (this.DownButton != null)
            {
                this.DownButton.Click -= OnDownClicked;
            }
            this.UpButton = this.GetTemplateChild("UpBtn") as RepeatButton;
            this.DownButton = this.GetTemplateChild("DnBtn") as RepeatButton;
            if (this.UpButton != null)
            {
                this.UpButton.Click += OnUpClicked;
            }
            if (this.DownButton != null)
            {
                this.DownButton.Click += OnDownClicked;
            }
        }
    }
}
