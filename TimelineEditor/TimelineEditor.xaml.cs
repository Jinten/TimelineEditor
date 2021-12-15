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
using Timeline.Controls;
using Timeline.Utilities;

namespace Timeline
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
            DependencyProperty.Register(nameof(TimelineLaneWidth), typeof(double), typeof(TimelineEditor), new FrameworkPropertyMetadata(800.0, TimelineLaneWidthPropertyChanged));

        public double UnitStep
        {
            get => (double)GetValue(UnitStepProperty);
            set => SetValue(UnitStepProperty, value);
        }
        public static readonly DependencyProperty UnitStepProperty =
            DependencyProperty.Register(nameof(UnitStep), typeof(double), typeof(TimelineEditor), new FrameworkPropertyMetadata(1.0, UnitStepPropertyChanged));

        public double CurrentValue
        {
            get => (double)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register(nameof(CurrentValue), typeof(double), typeof(TimelineEditor), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CurrentValuePropertyChanged));

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

        public DataTemplate TrackHeaderTemplate
        {
            get => (DataTemplate)GetValue(TrackHeaderTemplateProperty);
            set => SetValue(TrackHeaderTemplateProperty, value);
        }
        public static readonly DependencyProperty TrackHeaderTemplateProperty =
            DependencyProperty.Register(nameof(TrackHeaderTemplate), typeof(DataTemplate), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TrackHeaderTemplatePropertyChanged));

        public Style TrackListBoxStyle
        {
            get => (Style)GetValue(TrackListBoxStyleProperty);
            set => SetValue(TrackListBoxStyleProperty, value);
        }
        public static readonly DependencyProperty TrackListBoxStyleProperty =
            DependencyProperty.Register(nameof(TrackListBoxStyle), typeof(Style), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TrackListBoxStylePropertyChanged));

        public Style TrackItemContainerStyle
        {
            get => (Style)GetValue(TrackItemContainerStyleProperty);
            set => SetValue(TrackItemContainerStyleProperty, value);
        }
        public static readonly DependencyProperty TrackItemContainerStyleProperty =
            DependencyProperty.Register(nameof(TrackItemContainerStyle), typeof(Style), typeof(TimelineEditor), new FrameworkPropertyMetadata(null, TrackItemContainerStylePropertyChanged));

        public Brush TimelineLaneBackground
        {
            get => (Brush)GetValue(TimelineLaneBackgroundStyleProperty);
            set => SetValue(TimelineLaneBackgroundStyleProperty, value);
        }
        public static readonly DependencyProperty TimelineLaneBackgroundStyleProperty =
            DependencyProperty.Register(nameof(TimelineLaneBackground), typeof(Brush), typeof(TimelineEditor), new FrameworkPropertyMetadata(Brushes.Black, TimelineLaneBackgroundPropertyChanged));

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

        bool _IsKeyDragMoving = false;                          // KeyをDragで移動しているかどうかのフラグ
        bool _IsKeySelectionChanging = false;                   // KeyのIsSelectedが変更されて内部処理をしている状態かどうか
        bool _IsKeySelectWithMouseLeftButtonPushing = false;    // Key選択でMouseLeftButtonが押し続けられているかのフラグ
        TimelineKey? _DraggingKey = null;

        // 無駄な生成だけどLoadedで取得処理を書いても警告は消せないのでダミーで生成
        ScrollViewer _TrackListboxScrollViewer = new ScrollViewer();

        public TimelineEditor()
        {
            InitializeComponent();

            LaneWidthTextBox.Text = TimelineLaneWidth.ToString();

            LaneWidthTextBox.KeyDown += LaneWidthTextBox_KeyDown;
            LaneWidthTextBox.LostKeyboardFocus += LaneWidthTextBox_LostKeyboardFocus;

            // Rulerも含めた領域で検知するため、Gridでイベントを購読する
            TimelineLaneGrid.MouseLeftButtonDown += TimelineLaneMouseLeftButtonDown;

            TimelineLaneCanvas.MouseEnter += RaiseMousePositionOnTimelineLane;
            TimelineLaneCanvas.MouseMove += RaiseMousePositionOnTimelineLane;

            TimelineMarker.MarkerPositionChanged += MarkerPositionChanged;

            // UserControlのInitializeComponentが完了した直後だと、ListBoxの子階層のコントロールはまだ構築されていない。
            // Loadedイベント時点でUserControl内のコントロール要素の構築が完了しているので、子階層内部のScrollViewerが取得できる
            Loaded += (sender, e) =>
            {
                UpdateTimelineLaneWidth();

                _TrackListboxScrollViewer = FrameworkElementFinder.FirstFromChild<ScrollViewer>(TrackListBox);
                _TrackListboxScrollViewer.ScrollChanged += TrackListBox_ScrollChanged;

                TimelineLaneScrollViewer.SizeChanged += TimelineLaneScrollViewer_SizeChanged;
                TimelineLaneScrollViewer.ScrollChanged += TimelineLaneScrollViewer_ScrollChanged;

                // InputBinding
                TrackListBox.InputBindings.AddRange(TrackInputBindings);
                TimelineLaneCanvas.InputBindings.AddRange(LaneInputBindings);
            };

            Unloaded += (sender, e) =>
            {
                TimelineLaneCanvas.MouseMove -= RaiseMousePositionOnTimelineLane;
                TimelineLaneCanvas.MouseEnter -= RaiseMousePositionOnTimelineLane;

                LaneWidthTextBox.KeyDown -= LaneWidthTextBox_KeyDown;
                LaneWidthTextBox.LostKeyboardFocus -= LaneWidthTextBox_LostKeyboardFocus;

                TimelineLaneGrid.MouseLeftButtonDown -= TimelineLaneMouseLeftButtonDown;

                if (ItemsSource is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= ItemsSourceCollectionChanged;
                }

                _TrackListboxScrollViewer.ScrollChanged -= TrackListBox_ScrollChanged;

                TimelineLaneScrollViewer.ScrollChanged -= TimelineLaneScrollViewer_ScrollChanged;
            };
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            ContextMenuOpeningPosition = new Point(e.CursorLeft, e.CursorTop);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _IsKeySelectWithMouseLeftButtonPushing = false;
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                TimelineMarker.Visibility = Visibility.Collapsed;
            }
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

                _IsKeyDragMoving = false;
            }
            else if (TimelineMarker.Visibility == Visibility.Collapsed)
            {
                // シングルクリック時のみ処理
                // Key追加後であれば、LaneClickによる選択解除はしない（新規追加のKeyが選択状態の可能性があるため）
                if (LaneClickedCommand != null && LaneClickedCommand.CanExecute(e))
                {
                    LaneClickedCommand.Execute(e);
                }
            }

            // TimelineMarkerのVisibilityを見て、LaneClickedの判断をしてからCollapsedにする
            TimelineMarker.Visibility = Visibility.Collapsed;
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
            else if (IsKeyDownCtrl() && e.LeftButton == MouseButtonState.Pressed)
            {
                TimelineMarker.UpdatePosition(e.GetPosition(TimelineMarker).X);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsKeyDownCtrl() && Mouse.LeftButton == MouseButtonState.Pressed && _IsKeySelectWithMouseLeftButtonPushing == false)
            {
                if (_IsKeyDragMoving == false)
                {
                    TimelineMarker.UpdatePosition(Mouse.GetPosition(TimelineMarker).X);
                    TimelineMarker.Visibility = Visibility.Visible;
                }
            }
        }

        static void TimelineLaneWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;

            editor.UpdateTimelineLaneWidth();
        }

        static void UnitStepPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineRuler.UnitStep = (double)e.NewValue;
        }

        static void CurrentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineMarker.CurrentPosition = (double)e.NewValue * editor.TimelineRuler.UnitDistance;
        }

        static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;

            editor.TrackListBox.ItemsSource = (IEnumerable)e.NewValue;

            if (editor.TrackListBox.ItemsSource != null)
            {
                // 描画後のDispatcherPriorityに遅延させることで、コレクション整列後のUI位置を取得できるようにする
                // Normalだと、Renderよりも高いPriorityで実行されるため、新規追加されたItemはまだUI位置が確定していない。
                var items = editor.TrackListBox.ItemsSource.OfType<object>().ToArray();
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

        static bool IsKeyDownCtrl()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        static void TrackItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TrackListBox.ItemContainerStyle = (Style)e.NewValue;
        }

        static void TrackHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TrackHeader.ContentTemplate = (DataTemplate)e.NewValue;
        }

        static void TrackListBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TrackListBox.Style = (Style)e.NewValue;
            editor.TrackListBox.ApplyTemplate();
        }

        static void TimelineLaneBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TimelineLaneGrid.Background = (Brush)e.NewValue;
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

        static void TrackInputBindingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TimelineEditor)d;
            editor.TrackListBox.InputBindings.Clear();
            editor.TrackListBox.InputBindings.AddRange((InputBindingCollection)e.NewValue);
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
            if (_IsKeySelectWithMouseLeftButtonPushing)
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
                    if (_DraggingKey.IsSelected == false && IsKeyDownCtrl() == false)
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
            var pos = e.GetPosition(TimelineLaneCanvas);
            MousePositionOnTimelineLane = new Point(Math.Max(0, pos.X), pos.Y);
        }

        void MarkerPositionChanged(object? sender, double position)
        {
            CurrentValue = position / TimelineRuler.UnitDistance;
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
                laneCanvas.Width = TimelineLaneWidth;
            }

            TimelineRuler.Width = TimelineLaneWidth;
            TimelineLaneCanvas.Width = TimelineLaneWidth;
        }

        void ItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var items = TrackListBox.ItemContainerGenerator.Items.ToArray();

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
                var trackItem = (ListBoxItem)TrackListBox.ItemContainerGenerator.ContainerFromIndex(i);

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

                removeLane.KeySelectionChangedEvent -= TimelineKeySelectionChangedEventHandler;
                removeLane.KeyTouchEvent -= TimelineKeyTouchEventHandler;
                removeLane.KeyAddedEvent -= TimelineKeyAddedEventHandler;
                removeLane.KeyRemovedEvent -= TimelineKeyRemovedEventHandler;

                TimelineLaneCanvas.Children.Remove(removeLane);
            }

            // Track位置とLane位置を同期させる
            foreach (var laneCanvas in TimelineLaneCanvas.Children.OfType<TimelineLaneCanvas>())
            {
                var pos = laneCanvas.TrackItem.TranslatePoint(Define.ZeroPoint, TrackListBox);

                laneCanvas.SyncLaneVerticalPosition(pos.Y);
            }
        }

        TimelineLaneCanvas CreateLaneCanvas(ListBoxItem trackItem, object content)
        {
            var position = trackItem.TranslatePoint(Define.ZeroPoint, TrackListBox);
            var laneCanvas = new TimelineLaneCanvas(trackItem, TimelineKeyContainerStyle, content, 0, position.Y, TimelineLaneWidth);

            laneCanvas.Style = TimelineLaneContainerStyle;

            laneCanvas.KeyAddedEvent += TimelineKeyAddedEventHandler;
            laneCanvas.KeyRemovedEvent += TimelineKeyRemovedEventHandler;
            laneCanvas.KeyTouchEvent += TimelineKeyTouchEventHandler;
            laneCanvas.KeySelectionChangedEvent += TimelineKeySelectionChangedEventHandler;

            laneCanvas.ApplyTemplate();

            var keys = laneCanvas.GetTimelineKeys();
            SelectedTimelineKeyList.AddRange(keys.Where(arg => arg.IsSelected));

            return laneCanvas;
        }

        void TimelineLaneMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsKeyDownCtrl() && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(TimelineLaneScrollViewer);

                // スクロールバー上でMarkerは出さない
                if (0.0 <= pos.X && pos.X < TimelineLaneScrollViewer.ViewportWidth && -24 <= pos.Y && pos.Y < TimelineLaneScrollViewer.ViewportHeight)
                {
                    TimelineMarker.Visibility = Visibility.Visible;
                    TimelineMarker.CurrentPosition = e.GetPosition(TimelineMarker).X;

                    e.MouseDevice.Capture(this);
                }
            }
        }

        void TimelineKeyAddedEventHandler(object? sender, TimelineKey e)
        {
            if (e.IsSelected)
            {
                SelectedTimelineKeyList.Add(e);
            }
        }

        void TimelineKeyRemovedEventHandler(object? sender, TimelineKey e)
        {
            // もし選択中のKeyの中に削除されたKeyが含まれていたらリストから除外する
            SelectedTimelineKeyList.Remove(e);
        }

        void TimelineKeyTouchEventHandler(object? sender, TimelineKey key)
        {
            _IsKeySelectWithMouseLeftButtonPushing = true;

            // MouseLeftDownだけして選択はされていないが、Dragによって移動されるかもしれないKey
            // 移動し始めたら、このKeyも選択対象にする
            _DraggingKey = key;
        }

        void TimelineKeySelectionChangedEventHandler(object? sender, TimelineKey e)
        {
            if (_IsKeySelectionChanging)
            {
                return;
            }

            Keyboard.Focus(TimelineLaneCanvas);

            _IsKeySelectionChanging = true;

            if (IsKeyDownCtrl())
            {
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
                if (e.IsSelected)
                {
                    foreach (var key in SelectedTimelineKeyList)
                    {
                        key.IsSelected = false;
                    }
                    SelectedTimelineKeyList.Clear();

                    if (e.IsSelected == false)
                    {
                        throw new InvalidProgramException();
                    }
                    SelectedTimelineKeyList.Add(e);
                }
                else
                {
                    if (SelectedTimelineKeyList.Contains(e) == false)
                    {
                        throw new InvalidProgramException();
                    }
                    SelectedTimelineKeyList.Remove(e);
                }
            }

            _IsKeySelectionChanging = false;
        }

        void TrackListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            TimelineLaneCanvas.Height = e.ExtentHeight;
            TimelineLaneScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }

        void TimelineLaneScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TimelineRuler.InvalidateVisual();
        }

        void TimelineLaneScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            TimelineRuler.Offset = new Point(-e.HorizontalOffset, 0);
            _TrackListboxScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}