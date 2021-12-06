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

namespace TimelineEditor.Controls
{
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

        internal ListBoxItem TrackItem { get; }
        internal EventHandler<TimelineKey>? KeyRemovedEvent { get; set; }
        internal EventHandler<TimelineKey>? KeySelectEvent { get; set; }
        internal EventHandler<TimelineKey>? KeySelectedEvent { get;set; }

        TranslateTransform Translate { get; }

        Style? _KeyStyle;

        public TimelineLaneCanvas(ListBoxItem trackItem, Style? keyStyle, object content, double x, double y, double width)
        {
            ClipToBounds = true;

            TrackItem = trackItem;
            _KeyStyle = keyStyle;

            DataContext = content;

            TrackItem.SizeChanged += Track_SizeChanged;
            TrackItem.IsVisibleChanged += Track_IsVisibleChanged;

            Translate = new TranslateTransform()
            {
                X = x,
                Y = y
            };

            MinHeight = TrackItem.ActualHeight;
            MinWidth = width;

            var rand = new Random();
            byte r = (byte)rand.Next(0, 255);
            byte g = (byte)rand.Next(0, 255);
            byte b = (byte)rand.Next(0, 255);
            Background = new SolidColorBrush(Color.FromArgb(255, r, g, b));

            var group = new TransformGroup();
            group.Children.Add(Translate);

            RenderTransform = group;

            Unloaded += (sender, e) => 
            {
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

        internal void SyncLanePosition(double x, double y)
        {
            Translate.X = x;
            Translate.Y = y;
        }

        internal void UpdateKeyStyle(Style style)
        {
            foreach(var key in Children.OfType<TimelineKey>())
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

            var keys = ((IEnumerable)e.NewValue).OfType<object>().Select(arg => new TimelineKey(arg)).ToArray();

            foreach (var key in keys)
            {
                key.Style = lane._KeyStyle;
                key.ApplyTemplate();
            }

            lane.AddKeys(keys);

            if (e.OldValue is INotifyCollectionChanged oldNotifyCollectionChanged)
            {
                oldNotifyCollectionChanged.CollectionChanged -= lane.KeyCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newNotifyCollectionChanged)
            {
                newNotifyCollectionChanged.CollectionChanged += lane.KeyCollectionChanged;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                if(LaneDoubleClickedCommand != null && LaneDoubleClickedCommand.CanExecute(e))
                {
                    LaneDoubleClickedCommand.Execute(e);
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if(LaneClickedCommand != null && LaneClickedCommand.CanExecute(e))
            {
                LaneClickedCommand.Execute(e);
            }
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            ContextMenuOpeningPosition = new Point(e.CursorLeft, e.CursorTop);

            var expression = BindingOperations.GetBindingExpression(this, ContextMenuOpeningPositionProperty);
            expression?.UpdateSource();
        }

        void SubscribeTimelineKeyEvent(TimelineKey key)
        {
            key.SelectEvent += KeySelectEventHandler;
            key.SelectedEvent += KeySelectedEventHandler;
        }

        void UnsubscribeTimelineKeyEvent(TimelineKey key)
        {
            key.SelectedEvent -= KeySelectedEventHandler;
            key.SelectEvent -= KeySelectEventHandler;
        }

        void AddKeys(params TimelineKey[] addKeys)
        {
            foreach (var addKey in addKeys)
            {
                Children.Add(addKey);
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
            foreach(var removeKey in removeKeys)
            {
                Children.Remove(removeKey);
                KeyRemovedEvent?.Invoke(this, removeKey);
            }
        }

        void AddKeysInternal(object[] contents)
        {
            foreach (var content in contents)
            {
                var addKey = new TimelineKey(content)
                {
                    Style = _KeyStyle,
                    MinHeight = ActualHeight,
                    MaxHeight = ActualHeight,
                };

                SubscribeTimelineKeyEvent(addKey);

                addKey.ApplyTemplate();

                AddKeys(addKey);
            }
        }

        void RemoveKeysInternal(params object[] contents)
        {
            var removeKeys = Children.OfType<TimelineKey>().Where(arg => contents.Contains(arg.DataContext)).ToArray();
            RemoveKeys(removeKeys);
        }

        void KeySelectEventHandler(object? sender, TimelineKey key)
        {
            KeySelectEvent?.Invoke(this, key);
        }

        void KeySelectedEventHandler(object? sender, TimelineKey key)
        {
            KeySelectedEvent?.Invoke(this, key);
        }

        void Track_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        void Track_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MinHeight = e.NewSize.Height;
            MaxHeight = e.NewSize.Height;

            foreach (var key in Children.OfType<TimelineKey>())
            {
                key.MinHeight = MinHeight;
                key.MaxHeight = MinHeight;
            }
        }
    }
}
