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
    internal class TimelineRuler : ContentControl
    {
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(TimelineRuler), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double UnitStep
        {
            get => (double)GetValue(UnitStepProperty);
            set => SetValue(UnitStepProperty, value);
        }
        public static readonly DependencyProperty UnitStepProperty =
            DependencyProperty.Register(nameof(UnitStep), typeof(double), typeof(TimelineRuler), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double UnitDistance
        {
            get => (double)GetValue(UnitDistanceProperty);
            set => SetValue(UnitDistanceProperty, value);
        }
        public static readonly DependencyProperty UnitDistanceProperty =
            DependencyProperty.Register(nameof(UnitDistance), typeof(double), typeof(TimelineRuler), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public int SubUnitCount
        {
            get => (int)GetValue(SubUnitCountProperty);
            set => SetValue(SubUnitCountProperty, value);
        }
        public static readonly DependencyProperty SubUnitCountProperty =
            DependencyProperty.Register(nameof(SubUnitCount), typeof(int), typeof(TimelineRuler), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(Point), typeof(TimelineRuler), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(TimelineRuler), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, ColorPropertyChanged));

        public Brush SubColor
        {
            get => (Brush)GetValue(SubColorProperty);
            set => SetValue(SubColorProperty, value);
        }
        public static readonly DependencyProperty SubColorProperty =
            DependencyProperty.Register(nameof(SubColor), typeof(Brush), typeof(TimelineRuler), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, ColorPropertyChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(TimelineRuler), new FrameworkPropertyMetadata(Orientation.Horizontal));

        Pen? _LinePen = null;
        Pen? _SubLinePen = null;
        Pen? _BorderLeftPen = null;
        Pen? _BorderTopPen = null;
        Pen? _BorderRightPen = null;
        Pen? _BorderBottomPen = null;
        static Typeface Typeface { get; } = new Typeface("Verdana");
        static double LineOffset = 4;
        static double LineLength = 5 + LineOffset;
        static double SubLineOffset = 1;
        static double SubLineLength = 5 + SubLineOffset;
        static double TextHorizontalOffset = 2;

        static void ColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineRuler)d).UpdatePen(true);
        }

        static TimelineRuler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineRuler), new FrameworkPropertyMetadata(typeof(TimelineRuler)));
        }

        public TimelineRuler()
        {

        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            UpdatePen();
            UpdateBorderPen();

            dc.DrawRectangle(Background, null, new Rect(Define.ZeroPoint, new Point(ActualWidth, ActualHeight)));
            dc.DrawLine(_BorderLeftPen, Define.ZeroPoint, new Point(0, ActualHeight));
            dc.DrawLine(_BorderTopPen, Define.ZeroPoint, new Point(ActualWidth, 0));
            dc.DrawLine(_BorderRightPen, new Point(ActualWidth, 0), new Point(ActualWidth, ActualHeight));
            dc.DrawLine(_BorderBottomPen, new Point(0, ActualHeight), new Point(ActualWidth, ActualHeight));

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    OnRenderHorizontal(dc);
                    break;
                case Orientation.Vertical:
                    OnRenderVertical(dc);
                    break;
            }
        }

        void OnRenderHorizontal(DrawingContext dc)
        {
            double s = Math.Max(Scale, 1);

            // 親Gridの縦横を取得
            var parentGrid = (Grid)Parent;
            var parentActualWidth = parentGrid.ActualWidth;
            var parentActualHeight = parentGrid.ActualHeight;

            int init = Math.Max(0, (int)(-Offset.X / UnitDistance) - 1);
            int count = (int)((-Offset.X + parentActualWidth) / UnitDistance) + 1;

            double subLineDistance = UnitDistance / SubUnitCount;

            for (int i = init; i < count; ++i)
            {
                double num = i * UnitDistance;
                double x = (num + Offset.X) * s;
                dc.DrawLine(_LinePen, new Point(x, LineOffset), new Point(x, LineOffset + parentActualHeight));

                for (int j = 1; j < 10; ++j)
                {
                    double sub_x = x + j * subLineDistance * s;
                    dc.DrawLine(_SubLinePen, new Point(sub_x, SubLineOffset), new Point(sub_x, SubLineLength));
                }

                int numText = (int)(UnitStep * i);
                var text = new FormattedText($"{numText}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 8, Foreground, 1.0);
                dc.DrawText(text, new Point(x + TextHorizontalOffset, LineLength));
            }
        }

        void OnRenderVertical(DrawingContext dc)
        {
            double s = Math.Max(Scale, 1);
            double numScale = Math.Max(1.0 / Scale, 1);

            int init = (int)(-Offset.Y / UnitDistance) - 1;
            int count = (int)((-Offset.Y + ActualHeight) / UnitDistance) + 1;

            double subLineDistance = UnitDistance / SubUnitCount;

            for (int i = init; i < count; ++i)
            {
                double num = i * UnitDistance;
                double y = (num + Offset.Y) * s;
                dc.DrawLine(_LinePen, new Point(LineOffset, y), new Point(LineLength, y));

                for (int j = 1; j < 10; ++j)
                {
                    double sub_y = y + j * subLineDistance * s;
                    dc.DrawLine(_SubLinePen, new Point(SubLineOffset, sub_y), new Point(SubLineLength, sub_y));
                }

                int numText = (int)(num * numScale);
                var text = new FormattedText($"{numText}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 8, Foreground, 1.0);
                dc.DrawText(text, new Point(LineLength + LineOffset, y - text.Height * 0.5));
            }
        }

        void UpdatePen(bool remake = false)
        {
            if (_LinePen == null || remake)
            {
                _LinePen = new Pen(Color, 1);
                _LinePen.Freeze();
            }

            if (_SubLinePen == null || remake)
            {
                _SubLinePen = new Pen(SubColor, 1);
                _SubLinePen.Freeze();
            }
        }

        void UpdateBorderPen()
        {
            if (_BorderLeftPen == null)
            {
                _BorderLeftPen = new Pen(BorderBrush, BorderThickness.Left);
                _BorderLeftPen.Freeze();
            }

            if (_BorderTopPen == null)
            {
                _BorderTopPen = new Pen(BorderBrush, BorderThickness.Top);
                _BorderTopPen.Freeze();
            }

            if (_BorderRightPen == null)
            {
                _BorderRightPen = new Pen(BorderBrush, BorderThickness.Right);
                _BorderRightPen.Freeze();
            }

            if (_BorderBottomPen == null)
            {
                _BorderBottomPen = new Pen(BorderBrush, BorderThickness.Bottom);
                _BorderBottomPen.Freeze();
            }
        }
    }
}
