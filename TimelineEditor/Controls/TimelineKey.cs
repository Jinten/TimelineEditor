using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace TimelineEditor.Controls
{
    public class TimelineKey : ContentControl
    {
        public double PlacementPosition
        {
            get => (double)GetValue(PlacementPositionProperty);
            set => SetValue(PlacementPositionProperty, value);
        }
        public static readonly DependencyProperty PlacementPositionProperty =
            DependencyProperty.Register(nameof(PlacementPosition), typeof(double), typeof(TimelineKey),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PlacementPositionPropertyChanged));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(TimelineKey),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsSelectedPropertyChanged));

        internal EventHandler<TimelineKey>? SelectEvent { get; set; }
        internal EventHandler<TimelineKey>? SelectedEvent { get; set; }

        TranslateTransform Translate { get; }

        public TimelineKey(object content)
        {
            DataContext = content;

            Translate = new TranslateTransform();
            Translate.X = PlacementPosition;
            Translate.Y = 0;

            var group = new TransformGroup();
            group.Children.Add(Translate);

            RenderTransform = group;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // Ctrl+Clickによる超シビアなタイミングでClickCount==2で処理が来る場合がある
            // なので、ClickCountを見て処理を限定する実装はしないように。
            SelectEvent?.Invoke(this, this);
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            SelectedEvent?.Invoke(this, this);
            e.Handled = true;
        }

        static void PlacementPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var key = (TimelineKey)d;
            key.Translate.X = (double)e.NewValue;
        }

        static void IsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var key = (TimelineKey)d;
            key.SelectedEvent?.Invoke(key, key);
        }
    }
}
