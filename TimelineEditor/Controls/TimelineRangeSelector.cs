using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Timeline.Controls
{
    internal class TimelineRangeSelector : ContentControl
    {
        public double OriginPosition
        {
            get => (double)GetValue(OriginPositionProperty);
            set => SetValue(OriginPositionProperty, value);
        }
        public static readonly DependencyProperty OriginPositionProperty =
            DependencyProperty.Register(
                nameof(OriginPosition),
                typeof(double),
                typeof(TimelineRangeSelector),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OriginPositionPropertyChanged));

        public double CurrentPosition
        {
            get => (double)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }
        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register(
                nameof(CurrentPosition),
                typeof(double),
                typeof(TimelineRangeSelector),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, CurrentPositionPropertyChanged));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Brush),
                typeof(TimelineRangeSelector),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 0, 0, 0)),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    ColorPropertyChanged));

        Brush? _Color = null;

        public bool IsContentWithinRange(double min_pos, double max_pos)
        {
            var half_w = (max_pos - min_pos) * 0.5;
            var point_pos = min_pos + half_w;

            var min_x = Math.Min(CurrentPosition, OriginPosition) - half_w;
            var max_x = Math.Max(CurrentPosition, OriginPosition) + half_w;

            return min_x <= point_pos && point_pos <= max_x;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            UpdateColor();

            // 親のGridの高さを取得
            var parentCanvas = (Canvas)Parent;
            var parentActualHeight = parentCanvas.ActualHeight;

            var min_x = Math.Min(CurrentPosition, OriginPosition);
            var max_x = Math.Max(CurrentPosition, OriginPosition);
            dc.DrawRectangle(_Color, null, new Rect(min_x, 0, max_x - min_x, parentActualHeight));
        }

        static void OriginPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (TimelineRangeSelector)d;
        }

        static void CurrentPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (TimelineRangeSelector)d;
        }

        static void ColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (TimelineRangeSelector)d;
            selector.InvalidateVisual();
        }

        void UpdateColor(bool remake = false)
        {
            if (_Color == null || remake)
            {
                _Color = Color.Clone();
                _Color.Freeze();
            }
        }
    }
}
