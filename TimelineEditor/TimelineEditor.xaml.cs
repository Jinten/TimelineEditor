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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimelineEditor.Controls;
using TimelineEditor.Utilities;

namespace TimelineEditor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TimelineEditor : UserControl
    {
        public double TimelineLaneWidth
        {
            get => (double)GetValue(TimelineLaneWidthProperty);
            set => SetValue(TimelineLaneWidthProperty, value);
        }
        public static readonly DependencyProperty TimelineLaneWidthProperty =
            DependencyProperty.Register(nameof(TimelineLaneWidth), typeof(double), typeof(TimelineEditor), new FrameworkPropertyMetadata(1000.0, TimelineLaneWidthPropertyChanged));


        public Point MousePositionOnTimelineLane
        {
            get => (Point)GetValue(MousePositionOnTimelineLaneProperty);
            set => SetValue(MousePositionOnTimelineLaneProperty, value);
        }
        public static readonly DependencyProperty MousePositionOnTimelineLaneProperty =
            DependencyProperty.Register(nameof(MousePositionOnTimelineLane), typeof(Point), typeof(TimelineEditor), new FrameworkPropertyMetadata(Define.ZeroPoint, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, ItemsSourcePropertyChanged));


        public DataTemplate TrackItemTemplate
        {
            get => (DataTemplate)GetValue(TrackItemTemplateProperty);
            set => SetValue(TrackItemTemplateProperty, value);
        }
        public static readonly DependencyProperty TrackItemTemplateProperty =
            DependencyProperty.Register(nameof(TrackItemTemplate), typeof(DataTemplate), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TrackItemTemplatePropertyChanged));


        public Style TrackItemContainerStyle
        {
            get => (Style)GetValue(TrackItemContainerStyleProperty);
            set => SetValue(TrackItemContainerStyleProperty, value);
        }
        public static readonly DependencyProperty TrackItemContainerStyleProperty =
            DependencyProperty.Register(nameof(TrackItemContainerStyle), typeof(Style), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TrackItemContainerStylePropertyChanged));


        public Style TimelineLaneContainerStyle
        {
            get => (Style)GetValue(TimelineLaneContainerStyleProperty);
            set => SetValue(TimelineLaneContainerStyleProperty, value);
        }
        public static readonly DependencyProperty TimelineLaneContainerStyleProperty =
            DependencyProperty.Register(nameof(TimelineLaneContainerStyle), typeof(Style), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TimelineLaneContainerStylePropertyChanged));


        public Style TimelineKeyContainerStyle
        {
            get => (Style)GetValue(TimelineKeyContainerStyleProperty);
            set => SetValue(TimelineKeyContainerStyleProperty, value);
        }
        public static readonly DependencyProperty TimelineKeyContainerStyleProperty =
            DependencyProperty.Register(nameof(TimelineKeyContainerStyle), typeof(Style), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TimelineKeyContainerStylePropertyChanged));


        public ICommand BeginKeyMovingCommand
        {
            get => (ICommand)GetValue(BeginKeyMovingCommandProperty);
            set => SetValue(BeginKeyMovingCommandProperty, value);
        }
        public static readonly DependencyProperty BeginKeyMovingCommandProperty =
            DependencyProperty.Register(nameof(BeginKeyMovingCommand), typeof(ICommand), typeof(TimelineEditor), new FrameworkPropertyMetadata(null));


        public ICommand KeyMovingCommand
        {
            get => (ICommand)GetValue(KeyMovingCommandProperty);
            set => SetValue(KeyMovingCommandProperty, value);
        }
        public static readonly DependencyProperty KeyMovingCommandProperty =
            DependencyProperty.Register(nameof(KeyMovingCommand), typeof(ICommand), typeof(TimelineEditor), new FrameworkPropertyMetadata(null));


        public ICommand EndKeyMovingCommand
        {
            get => (ICommand)GetValue(EndKeyMovingCommandProperty);
            set => SetValue(EndKeyMovingCommandProperty, value);
        }
        public static readonly DependencyProperty EndKeyMovingCommandProperty =
            DependencyProperty.Register(nameof(EndKeyMovingCommand), typeof(ICommand), typeof(TimelineEditor), new FrameworkPropertyMetadata(null));


        public ICommand LaneClickedCommand
        {
            get => (ICommand)GetValue(LaneClickedCommandProperty);
            set => SetValue(LaneClickedCommandProperty, value);
        }
        public static readonly DependencyProperty LaneClickedCommandProperty =
            DependencyProperty.Register(nameof(LaneClickedCommand), typeof(ICommand), typeof(TimelineEditor), new FrameworkPropertyMetadata(null));


        public Point ContextMenuOpeningPosition
        {
            get => (Point)GetValue(ContextMenuOpeningPositionProperty);
            set => SetValue(ContextMenuOpeningPositionProperty, value);
        }
        public static readonly DependencyProperty ContextMenuOpeningPositionProperty =
            DependencyProperty.Register(nameof(ContextMenuOpeningPosition), typeof(Point), typeof(TimelineEditor), new FrameworkPropertyMetadata(Define.ZeroPoint, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public InputBindingCollection TrackInputBindings
        {
            get => (InputBindingCollection)GetValue(TrackInputBindingsProperty);
            set => SetValue(TrackInputBindingsProperty, value);
        }
        public static readonly DependencyProperty TrackInputBindingsProperty =
            DependencyProperty.Register(nameof(TrackInputBindings), typeof(InputBindingCollection), typeof(TimelineEditor), new FrameworkPropertyMetadata(new InputBindingCollection(), TrackInputBindingsPropertyChanged));

        public InputBindingCollection LaneInputBindings
        {
            get => (InputBindingCollection)GetValue(LaneInputBindingsProperty);
            set => SetValue(LaneInputBindingsProperty, value);
        }
        public static readonly DependencyProperty LaneInputBindingsProperty =
            DependencyProperty.Register(nameof(LaneInputBindings), typeof(InputBindingCollection), typeof(TimelineEditor), new FrameworkPropertyMetadata(new InputBindingCollection(), LaneInputBindingsPropertyChanged));


        List<TimelineKey> SelectedTimelineKeyList { get; } = new List<TimelineKey>();

        bool _IsKeySelecting = false;
        bool _IsKeyDragMoving = false;
        bool _IsKeySelectionChanging = false;
        TimelineKey? _DraggingKey = null;

        // 無駄な生成だけどLoadedで取得処理を書いても警告は消せないのでダミーで生成
        ScrollViewer _TrackListboxScrollViewer = new ScrollViewer();

        public TimelineEditor()
        {
            InitializeComponent();

            LaneWidthTextBox.Text = TimelineLaneWidth.ToString();

            LaneWidthTextBox.KeyDown += LaneWidthTextBox_KeyDown;
            LaneWidthTextBox.LostKeyboardFocus += LaneWidthTextBox_LostKeyboardFocus;

            TimelineLaneCanvas.MouseEnter += RaiseMousePositionOnTimelineLane;
            TimelineLaneCanvas.MouseMove += RaiseMousePositionOnTimelineLane;

            // UserControlのInitializeComponentが完了した直後だと、ListBoxの子階層のコントロールはまだ構築されていない。
            // Loadedイベント時点でUserControl内のコントロール要素の構築が完了しているので、子階層内部のScrollViewerが取得できる
            Loaded += (sender, e) =>
            {
                _TrackListboxScrollViewer = FrameworkElementFinder.FirstFromChild<ScrollViewer>(TimelineTrackListBox);
                _TrackListboxScrollViewer.ScrollChanged += TimelineTrackListBox_ScrollChanged;

                TimelineLaneScrollViewer.ScrollChanged += TimelineLaneScrollViewer_ScrollChanged;

                // InputBinding
                TimelineTrackListBox.InputBindings.AddRange(TrackInputBindings);
                TimelineLaneCanvas.InputBindings.AddRange(LaneInputBindings);
            };

            Unloaded += (sender, e) =>
            {
                TimelineLaneCanvas.MouseMove -= RaiseMousePositionOnTimelineLane;
                TimelineLaneCanvas.MouseEnter -= RaiseMousePositionOnTimelineLane;

                LaneWidthTextBox.KeyDown -= LaneWidthTextBox_KeyDown;
                LaneWidthTextBox.LostKeyboardFocus -= LaneWidthTextBox_LostKeyboardFocus;

                if (ItemsSource is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= ItemsSourceCollectionChanged;
                }

                _TrackListboxScrollViewer.ScrollChanged -= TimelineTrackListBox_ScrollChanged;

                TimelineLaneScrollViewer.ScrollChanged -= TimelineLaneScrollViewer_ScrollChanged;
            };
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            ContextMenuOpeningPosition = new Point(e.CursorLeft, e.CursorTop);

            var expression = BindingOperations.GetBindingExpression(this, ContextMenuOpeningPositionProperty);
            expression?.UpdateSource();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();

            // DragによるKey移動をしていたなら、選択はそのままにしておく
            if (_IsKeyDragMoving)
            {
                // Mouseの左ボタンがUpされたので、関連処理のフラグを全てクリア
                if (EndKeyMovingCommand != null && EndKeyMovingCommand.CanExecute(MousePositionOnTimelineLane))
                {
                    EndKeyMovingCommand.Execute(MousePositionOnTimelineLane);
                }

                _IsKeySelecting = false;
                _IsKeyDragMoving = false;
                return;
            }
            else
            {
                if (LaneClickedCommand != null && LaneClickedCommand.CanExecute(e))
                {
                    LaneClickedCommand.Execute(e);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_IsKeyDragMoving)
            {
                if (KeyMovingCommand != null && KeyMovingCommand.CanExecute(MousePositionOnTimelineLane))
                {
                    KeyMovingCommand.Execute(MousePositionOnTimelineLane);
                }

                UpdatePositionOnTimelineLane(e);
            }
        }

        static void TimelineLaneWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;

            editor.UpdateTimelineLaneWidth();
        }

        static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;

            editor.TimelineTrackListBox.ItemsSource = (IEnumerable)e.NewValue;

            if (editor.TimelineTrackListBox.ItemsSource != null)
            {
                // 描画後のDispatcherPriorityに遅延させることで、コレクション整列後のUI位置を取得できるようにする
                // Normalだと、Renderよりも高いPriorityで実行されるため、新規追加されたItemはまだUI位置が確定していない。
                var items = editor.TimelineTrackListBox.ItemsSource.OfType<object>().ToArray();
                Application.Current.Dispatcher.InvokeAsync(() => editor.TreeViewItemCollectionRenderCommitted(items), DispatcherPriority.Input);
            }

            if (e.OldValue is INotifyCollectionChanged oldNotifyCollection)
            {
                oldNotifyCollection.CollectionChanged -= editor.ItemsSourceCollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += editor.ItemsSourceCollectionChanged;
            }
        }

        static void TrackItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineTrackListBox.ItemContainerStyle = (Style)e.NewValue;
        }

        static void TimelineLaneContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            var laneCanvasList = editor.TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>().ToArray();

            foreach (var laneCanvas in laneCanvasList)
            {
                laneCanvas.Style = (Style)e.NewValue;
                laneCanvas.ApplyTemplate();
            }
        }

        static void TimelineKeyContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            var laneCanvasList = editor.TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>().ToArray();

            foreach (var laneCanvas in laneCanvasList)
            {
                laneCanvas.UpdateKeyStyle((Style)e.NewValue);
            }
        }

        static void TrackItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineTrackListBox.ItemTemplate = (DataTemplate)e.NewValue;
        }

        static void TrackInputBindingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineTrackListBox.InputBindings.Clear();
            editor.TimelineTrackListBox.InputBindings.AddRange((InputBindingCollection)e.NewValue);
        }

        static void LaneInputBindingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineLaneCanvas.InputBindings.Clear();
            editor.TimelineLaneCanvas.InputBindings.AddRange((InputBindingCollection)e.NewValue);
        }

        void LaneWidthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ParseLaneWidthText();
                UpdateTimelineLaneWidth();

                LaneWidthTextBox.SelectAll();
            }
        }

        void LaneWidthTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ParseLaneWidthText();
            UpdateTimelineLaneWidth();
        }

        void RaiseMousePositionOnTimelineLane(object sender, MouseEventArgs e)
        {
            // Keyを選択中に（選択してからMouse左クリックしながら）移動していれば、Keyの座標移動と見なす
            if (_IsKeySelecting)
            {
                if (_DraggingKey == null)
                {
                    throw new InvalidProgramException();
                }

                // KeyをMouseLeftDownしたままDragし始めたら、選択候補のこのKeyも選択してからKey移動を開始する

                _IsKeySelectionChanging = true; // SelectionChangedを着火しないようにする
                {
                    // Drag開始Keyが非選択でCtrlが押されていなければ、Drag開始したKeyのみ選択して移動対象とする
                    // 逆に言えば、Drag開始Keyが選択されていれば、それも含めて一緒に移動させる
                    if (_DraggingKey.IsSelected == false && Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.RightCtrl) == false)
                    {
                        SelectedTimelineKeyList.ForEach(arg => arg.IsSelected = false);
                        SelectedTimelineKeyList.Clear();
                    }

                    _DraggingKey.IsSelected = true;
                    if (SelectedTimelineKeyList.Contains(_DraggingKey) == false)
                    {
                        SelectedTimelineKeyList.Add(_DraggingKey);
                    }
                }
                _IsKeySelectionChanging = false;

                if (_IsKeyDragMoving == false)
                {
                    e.MouseDevice.Capture(this);

                    // KeyのDrag移動を始めて処理するので、関連Comman着火とフラグ制御
                    if (BeginKeyMovingCommand != null && BeginKeyMovingCommand.CanExecute(MousePositionOnTimelineLane))
                    {
                        BeginKeyMovingCommand.Execute(MousePositionOnTimelineLane);
                    }
                    _IsKeyDragMoving = true;

                    Keyboard.Focus(TimelineLaneCanvas);
                }
            }

            UpdatePositionOnTimelineLane(e);
        }

        void UpdatePositionOnTimelineLane(MouseEventArgs e)
        {
            MousePositionOnTimelineLane = e.GetPosition(TimelineLaneCanvas);

            var expression = BindingOperations.GetBindingExpression(this, MousePositionOnTimelineLaneProperty);
            expression?.UpdateSource();
        }

        void ParseLaneWidthText()
        {
            if (double.TryParse(LaneWidthTextBox.Text, out double trackLaneWidth))
            {
                TimelineLaneWidth = trackLaneWidth;
            }
            else
            {
                LaneWidthTextBox.Text = TimelineLaneWidth.ToString();
            }
        }

        void UpdateTimelineLaneWidth()
        {
            foreach (var laneCanvas in TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>())
            {
                laneCanvas.MinWidth = TimelineLaneWidth;
            }
        }

        void ItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var items = TimelineTrackListBox.ItemContainerGenerator.Items.ToArray();

            // 描画後のDispatcherPriorityに遅延させることで、コレクション整列後のUI位置を取得できるようにする
            // Normalだと、Renderよりも高いPriorityで実行されるため、新規追加されたItemはまだUI位置が確定していない。
            Application.Current.Dispatcher.InvokeAsync(() => TreeViewItemCollectionRenderCommitted(items), DispatcherPriority.Loaded);
        }

        void TreeViewItemCollectionRenderCommitted(object[] contents)
        {
            var oldLaneCanvasList = TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>().ToArray();

            for (int i = 0; i < contents.Length; i++)
            {
                // 構成されたTreeViewItemを取得して、描画されている位置を取得する
                var trackItem = (ListBoxItem)TimelineTrackListBox.ItemContainerGenerator.ContainerFromIndex(i);

                var content = contents[i];
                if (i >= oldLaneCanvasList.Length)
                {
                    // 既存要素数を超えているので、新規追加されるコンテンツ
                    var lane = CreateLaneCanvas(trackItem, content);

                    TimelineLaneCanvas.Children.Add(lane);
                }
                else if (oldLaneCanvasList[i].DataContext != content)
                {
                    var existingLane = oldLaneCanvasList.FirstOrDefault(arg => arg.DataContext == content);
                    if (existingLane != null)
                    {
                        // 順番が変わった場合は、順序入れ替えを行う
                        TimelineLaneCanvas.Children.Remove(existingLane);
                        TimelineLaneCanvas.Children.Insert(i, existingLane);
                    }
                    else
                    {
                        // Laneにない場合は新規追加
                        // 既存要素数を超えているので、新規追加されるコンテンツ
                        var lane = CreateLaneCanvas(trackItem, content);

                        TimelineLaneCanvas.Children.Insert(i, lane);
                    }
                }
            }

            var newLaneCanvasList = TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>().ToArray();

            // この時点で必ずLaneCanvasは1つ以上存在する
            for (int i = newLaneCanvasList.Length; i > contents.Length; --i)
            {
                TimelineLaneCanvas removeLane = newLaneCanvasList[i - 1];

                var keys = removeLane.GetTimelineKeys();
                foreach (var key in keys)
                {
                    SelectedTimelineKeyList.Remove(key);
                }

                removeLane.KeySelectedEvent -= TimelineKeySelectedEventHandler;
                removeLane.KeySelectEvent -= TimelineKeySelectEventHandler;
                removeLane.KeyRemovedEvent -= TimelineKeyRemovedEventHandler;

                TimelineLaneCanvas.Children.Remove(removeLane);
            }

            // Track位置とLane位置を同期させる
            foreach (var laneCanvas in TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>())
            {
                var pos = laneCanvas.TrackItem.TranslatePoint(Define.ZeroPoint, TimelineTrackListBox);

                laneCanvas.SyncLanePosition(pos.X, pos.Y);
            }
        }

        TimelineLaneCanvas CreateLaneCanvas(ListBoxItem trackItem, object content)
        {
            var position = trackItem.TranslatePoint(Define.ZeroPoint, TimelineTrackListBox);
            var laneCanvas = new TimelineLaneCanvas(trackItem, TimelineKeyContainerStyle, content, 0.0, position.Y, TimelineLaneWidth);

            laneCanvas.Style = TimelineLaneContainerStyle;

            laneCanvas.KeyRemovedEvent += TimelineKeyRemovedEventHandler;
            laneCanvas.KeySelectEvent += TimelineKeySelectEventHandler;
            laneCanvas.KeySelectedEvent += TimelineKeySelectedEventHandler;

            laneCanvas.ApplyTemplate();

            return laneCanvas;
        }

        void TimelineKeyRemovedEventHandler(object? sender, TimelineKey e)
        {
            // もし選択中のKeyの中に削除されたKeyが含まれていたらリストから除外する
            SelectedTimelineKeyList.Remove(e);
        }

        void TimelineKeySelectEventHandler(object? sender, TimelineKey key)
        {
            _IsKeySelecting = true;

            // MouseLeftDownだけして選択はされていないが、Dragによって移動されるかもしれないKey
            // 移動し始めたら、このKeyも選択対象にする
            _DraggingKey = key;
        }

        void TimelineKeySelectedEventHandler(object? sender, TimelineKey e)
        {
            if (_IsKeySelectionChanging)
            {
                return;
            }

            Keyboard.Focus(TimelineLaneCanvas);

            _IsKeySelecting = false;

            _IsKeySelectionChanging = true;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.IsSelected = !e.IsSelected;

                if (e.IsSelected)
                {
                    SelectedTimelineKeyList.Add(e);
                }
                else
                {
                    SelectedTimelineKeyList.Remove(e);
                }
            }
            else
            {
                foreach (var key in SelectedTimelineKeyList)
                {
                    key.IsSelected = false;
                }
                SelectedTimelineKeyList.Clear();

                e.IsSelected = true;

                SelectedTimelineKeyList.Add(e);
            }

            _IsKeySelectionChanging = false;
        }

        void TimelineTrackListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            TimelineLaneCanvas.Height = e.ExtentHeight;
            TimelineLaneScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }

        void TimelineLaneScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _TrackListboxScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}