using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Timeline.Controls
{
    internal class TimelineMarker : ContentControl
    {
        public double CurrentPosition
        {
            get => (double)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }
        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register(nameof(CurrentPosition), typeof(double), typeof(TimelineMarker), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush LineColor
        {
            get => (Brush)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(nameof(LineColor), typeof(Brush), typeof(TimelineMarker), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, ColorPropertyChanged));

        Pen? _LinePen = null;
        static double BackWidth = 17;
        static Brush BackColor { get; } = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));

        static void ColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineMarker)d).UpdatePen(true);
        }

        static TimelineMarker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineMarker), new FrameworkPropertyMetadata(typeof(TimelineMarker)));
        }

        public TimelineMarker()
        {

        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            UpdatePen();

            dc.DrawRectangle(Background, null, new Rect(Define.ZeroPoint, new Point(ActualWidth, ActualHeight)));

            // 親のGridの高さを取得
            var parentGrid = (Grid)Parent;
            var parentActualHeight = parentGrid.ActualHeight;

            dc.DrawRectangle(BackColor, null, new Rect(CurrentPosition - BackWidth * 0.5, 0, BackWidth, parentActualHeight));

            dc.DrawLine(_LinePen, new Point(CurrentPosition, 0), new Point(CurrentPosition, parentActualHeight));
        }

        void UpdatePen(bool remake = false)
        {
            if (_LinePen == null || remake)
            {
                _LinePen = new Pen(LineColor, 1);
                _LinePen.Freeze();
            }
        }
    }
}
