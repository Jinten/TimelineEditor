using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Timeline.Controls
{
    internal class KeyMouseEventArgs : EventArgs
    {
        internal TimelineKey Key { get; }
        internal MouseEventArgs EventArgs { get; }

        internal KeyMouseEventArgs(TimelineKey key, MouseEventArgs eventArgs)
        {
            Key = key;
            EventArgs = eventArgs;
        }
    }

    public class TimelineLaneCanvas : Canvas
    {
        public IEnumerable KeysSource
        {
            get => (IEnumerable)GetValue(KeysSourceProperty);
            set => SetValue(KeysSourceProperty, value);
        }
        public static readonly DependencyProperty KeysSourceProperty =
            DependencyProperty.Register(nameof(KeysSource), typeof(IEnumerable), typeof(TimelineLaneCanvas), new FrameworkPropertyMetadata(null, KeysSourcePropertyChanged));

        public ICommand LaneDoubleClickedCommand
        {
            get => (ICommand)GetValue(LaneDoubleClickedCommandProperty);
            set => SetValue(LaneDoubleClickedCommandProperty, value);
        }
        public static readonly DependencyProperty LaneDoubleClickedCommandProperty =
            DependencyProperty.Register(nameof(LaneDoubleClickedCommand), typeof(ICommand), typeof(TimelineLaneCanvas), new FrameworkPropertyMetadata(null));

        public ICommand LaneClickedCommand
        {
            get => (ICommand)GetValue(LaneClickedCommandProperty);
            set => SetValue(LaneClickedCommandProperty, value);
        }
        public static readonly DependencyProperty LaneClickedCommandProperty =
            DependencyProperty.Register(nameof(LaneClickedCommand), typeof(ICommand), typeof(TimelineLaneCanvas), new FrameworkPropertyMetadata(null));

        public Point ContextMenuOpeningPosition
        {
            get => (Point)GetValue(ContextMenuOpeningPositionProperty);
            set => SetValue(ContextMenuOpeningPositionProperty, value);
        }
        public static readonly DependencyProperty ContextMenuOpeningPositionProperty =
            DependencyProperty.Register(nameof(ContextMenuOpeningPosition), typeof(Point), typeof(TimelineLaneCanvas), new FrameworkPropertyMetadata(Define.ZeroPoint));

        public Brush UnderBorder
        {
            get => (Brush)GetValue(UnderBorderProperty);
            set => SetValue(UnderBorderProperty, value);
        }
        public static readonly DependencyProperty UnderBorderProperty =
            DependencyProperty.Register(nameof(UnderBorder), typeof(Brush), typeof(TimelineLaneCanvas), new FrameworkPropertyMetadata(null, UnderBorderPropertyChanged));

        internal ListBoxItem TrackItem { get; }
        internal EventHandler<TimelineKey>? KeyAddedEvent { get; set; }
        internal EventHandler<TimelineKey>? KeyRemovedEvent { get; set; }
        internal EventHandler<TimelineKey>? KeySelectionChangedEvent { get; set; }
        internal EventHandler<KeyMouseEventArgs>? PreKeyMouseLeftBottomDownEvent { get; set; }
        internal EventHandler<KeyMouseEventArgs>? PreKeyMouseLeftBottomUpEvent { get; set; }

        TranslateTransform Translate { get; }

        Style? _KeyStyle;
        Pen? _UnderBorderPen;

        public TimelineLaneCanvas(ListBoxItem trackItem, Style? keyStyle, object content, double x, double y, double width)
        {
            ClipToBounds = true;

            TrackItem = trackItem;
            _KeyStyle = keyStyle;

            DataContext = content;

            SizeChanged += TimelineLaneCanvas_SizeChanged;

            TrackItem.SizeChanged += Track_SizeChanged;
            TrackItem.IsVisibleChanged += Track_IsVisibleChanged;

            Translate = new TranslateTransform()
            {
                X = x,
                Y = y
            };

            MinHeight = TrackItem.ActualHeight;
            MinWidth = width;

            var group = new TransformGroup();
            group.Children.Add(Translate);

            RenderTransform = group;

            Unloaded += (sender, e) =>
            {
                SizeChanged -= TimelineLaneCanvas_SizeChanged;

                foreach (var key in Children.OfType<TimelineKey>())
                {
                    UnsubscribeTimelineKeyEvent(key);
                }
                Children.Clear();

                if (KeysSource is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -= KeyCollectionChanged;
                }
            };
        }

        internal TimelineKey[] GetTimelineKeys()
        {
            return Children.OfType<TimelineKey>().ToArray();
        }

        internal void SyncLaneVerticalPosition(double y)
        {
            Translate.Y = y;
        }

        internal void UpdateKeyStyle(Style style)
        {
            foreach (var key in Children.OfType<TimelineKey>())
            {
                key.Style = style;
                key.ApplyTemplate();
            }

            _KeyStyle = style;
        }

        static void KeysSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lane = (TimelineLaneCanvas)d;
            var removeKeys = lane.Children.OfType<TimelineKey>().ToArray();

            foreach (var removeKey in removeKeys)
            {
                lane.RemoveKeys(removeKey);
            }

            var contents = ((IEnumerable)e.NewValue).OfType<object>().ToArray();
            lane.AddKeysInternal(contents);

            if (e.OldValue is INotifyCollectionChanged oldNotifyCollectionChanged)
            {
                oldNotifyCollectionChanged.CollectionChanged -= lane.KeyCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newNotifyCollectionChanged)
            {
                newNotifyCollectionChanged.CollectionChanged += lane.KeyCollectionChanged;
            }
        }

        static void UnderBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lane = (TimelineLaneCanvas)d;
            lane._UnderBorderPen = new Pen((Brush)e.NewValue, 1);
            lane._UnderBorderPen.Freeze();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (e.Handled == false)
                {
                    if (LaneDoubleClickedCommand != null && LaneDoubleClickedCommand.CanExecute(e))
                    {
                        LaneDoubleClickedCommand.Execute(e);
                    }
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (LaneClickedCommand != null && LaneClickedCommand.CanExecute(e))
            { 
                LaneClickedCommand.Execute(e);
            }
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            ContextMenuOpeningPosition = new Point(e.CursorLeft, e.CursorTop);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight-1));
            dc.DrawLine(_UnderBorderPen, new Point(0, ActualHeight), new Point(ActualWidth, ActualHeight));
        }

        void SubscribeTimelineKeyEvent(TimelineKey key)
        {
            key.PreMouseLeftButtonDown += KeyPreMouseLeftButtonDownEventHandler;
            key.PreMouseLeftButtonUp += KeyPreMouseLeftButtonUpEventHandler;
            key.SelectionChangedEvent += KeySelectionChangedEventHandler;
        }

        void UnsubscribeTimelineKeyEvent(TimelineKey key)
        {
            key.SelectionChangedEvent -= KeySelectionChangedEventHandler;
            key.PreMouseLeftButtonUp -= KeyPreMouseLeftButtonUpEventHandler;
            key.PreMouseLeftButtonDown -= KeyPreMouseLeftButtonDownEventHandler;
        }

        void AddKeysInternal(params TimelineKey[] addKeys)
        {
            foreach (var addKey in addKeys)
            {
                addKey.Style = _KeyStyle;

                // LaneCanvasに1pxの下線を引くため
                addKey.MinHeight = Math.Max(0, ActualHeight - 1);
                addKey.MaxHeight = Math.Max(0, ActualHeight - 1);

                addKey.ApplyTemplate();

                SubscribeTimelineKeyEvent(addKey);

                Children.Add(addKey);

                KeyAddedEvent?.Invoke(this, addKey);
            }
        }

        void RemoveKeys(params TimelineKey[] removeKeys)
        {
            foreach (var removeKey in removeKeys)
            {
                UnsubscribeTimelineKeyEvent(removeKey);

                Children.Remove(removeKey);

                KeyRemovedEvent?.Invoke(this, removeKey);
            }
        }

        void KeyCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                RemoveKeysInternal(e.OldItems.OfType<object>().ToArray());
            }

            if (e.NewItems != null)
            {
                AddKeysInternal(e.NewItems.OfType<object>().ToArray());
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearKeysInternal();
            }
        }

        void ClearKeysInternal()
        {
            var removeKeys = Children.OfType<TimelineKey>().ToArray();
            foreach (var removeKey in removeKeys)
            {
                Children.Remove(removeKey);
                KeyRemovedEvent?.Invoke(this, removeKey);
            }
        }

        void AddKeysInternal(object[] contents)
        {
            foreach (var content in contents)
            {
                AddKeysInternal(new TimelineKey(content));
            }
        }

        void RemoveKeysInternal(params object[] contents)
        {
            var removeKeys = Children.OfType<TimelineKey>().Where(arg => contents.Contains(arg.DataContext)).ToArray();
            RemoveKeys(removeKeys);
        }

        void KeyPreMouseLeftButtonDownEventHandler(object? sender, MouseButtonEventArgs e)
        {
            if(sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }
            TimelineKey key = (TimelineKey)sender;

            PreKeyMouseLeftBottomDownEvent?.Invoke(this, new KeyMouseEventArgs(key, e));
        }

        void KeyPreMouseLeftButtonUpEventHandler(object? sender, MouseButtonEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }
            TimelineKey key = (TimelineKey)sender;

            PreKeyMouseLeftBottomUpEvent?.Invoke(this, new KeyMouseEventArgs(key, e));
        }

        void KeySelectionChangedEventHandler(object? sender, TimelineKey key)
        {
            KeySelectionChangedEvent?.Invoke(this, key);
        }

        void TimelineLaneCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var key in Children.OfType<TimelineKey>())
            {
                key.MinHeight = ActualHeight - 1;
                key.MaxHeight = ActualHeight - 1;
            }
        }

        void Track_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        void Track_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = e.NewSize.Height;
        }
    }
}
