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
            DependencyProperty.Register(nameof(CurrentPosition), typeof(double), typeof(TimelineMarker), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, CurrentPositionPropertyChanged));

        public Brush LineColor
        {
            get => (Brush)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(nameof(LineColor), typeof(Brush), typeof(TimelineMarker), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, LineColorPropertyChanged));

        public Brush BackColor
        {
            get => (Brush)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register(nameof(BackColor), typeof(Brush), typeof(TimelineMarker), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(40,0,0,0)), FrameworkPropertyMetadataOptions.AffectsRender, BackColorPropertyChanged));

        public double BackWidth
        {
            get => (double)GetValue(BackWidthProperty);
            set => SetValue(BackWidthProperty, value);
        }
        public static readonly DependencyProperty BackWidthProperty =
            DependencyProperty.Register(nameof(BackWidth), typeof(double), typeof(TimelineMarker), new FrameworkPropertyMetadata(DefaultBackWidth, FrameworkPropertyMetadataOptions.AffectsRender));

        internal EventHandler<double>? MarkerPositionChanged;

        Pen? _LinePen = null;
        Brush? _BackColor = null;
        const double DefaultBackWidth = 17;


        static TimelineMarker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineMarker), new FrameworkPropertyMetadata(typeof(TimelineMarker)));
        }

        public TimelineMarker()
        {

        }

        internal void UpdatePosition(double pos)
        {
            CurrentPosition = Math.Max(0, pos);
        }

        internal void UpdatePositionFromValue(double value)
        {
            CurrentPosition = value;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            UpdatePen();
            UpdateBackColor();

            // 親のGridの高さを取得
            var parentGrid = (Grid)Parent;
            var parentActualHeight = parentGrid.ActualHeight;

            dc.DrawRectangle(_BackColor, null, new Rect(CurrentPosition - BackWidth * 0.5, 0, BackWidth, parentActualHeight));

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

        void UpdateBackColor(bool remake = false)
        {
            if (_BackColor == null || remake)
            {
                _BackColor = BackColor.Clone();
                _BackColor.Freeze();
            }
        }

        static void CurrentPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineMarker)d;
            editor.MarkerPositionChanged?.Invoke(editor, (double)e.NewValue);
        }

        static void LineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineMarker)d).UpdatePen(true);
        }

        static void BackColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineMarker)d).UpdateBackColor(true);
        }
    }
}
