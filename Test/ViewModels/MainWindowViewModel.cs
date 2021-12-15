using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Test.Utilities;

namespace Test.ViewModels
{
    public class TimelineKeyViewModel : ViewModel
    {
        public double PlacementPosition
        {
            get => _PlacementPosition;
            set => RaisePropertyChangedIfSet(ref _PlacementPosition, value);
        }
        double _PlacementPosition = 0;

        public bool IsSelected
        {
            get => _IsSelected;
            set => RaisePropertyChangedIfSet(ref _IsSelected, value);
        }
        bool _IsSelected = true;

        public TimelineKeyViewModel(double position)
        {
            PlacementPosition = position;
        }
    }

    public class TrackItemViewModel : ViewModel
    {
        public string Name
        {
            get => _Name;
            set => RaisePropertyChangedIfSet(ref _Name, value);
        }
        string _Name = string.Empty;

        public bool IsSelected
        {
            get => _IsSelected;
            set => RaisePropertyChangedIfSet(ref _IsSelected, value);
        }
        bool _IsSelected;

        public IEnumerable<TimelineKeyViewModel> Keys => _Keys;
        ObservableCollection<TimelineKeyViewModel> _Keys = new ObservableCollection<TimelineKeyViewModel>();

        public ViewModelCommand AddKeyCommand => _AddKeyCommand.Get(AddKey);
        ViewModelCommandHandler _AddKeyCommand = new ViewModelCommandHandler();

        public ViewModelCommand AddKeyWithDoubleClickCommand => _AddKeyWithDoubleClickCommand.Get(AddKeyWithDoubleClick);
        ViewModelCommandHandler _AddKeyWithDoubleClickCommand = new ViewModelCommandHandler();
        

        MainWindowViewModel Owner { get; }

        public TrackItemViewModel(string name, MainWindowViewModel owner)
        {
            Name = name;
            Owner = owner;

            CompositeDisposable.Add(() =>
            {
                foreach(var key in _Keys)
                {
                    key.Dispose();
                }
            });
        }

        public void AddKey()
        {
            var key = new TimelineKeyViewModel(Owner.ContextMenuOpeningPosition.X);
            AddKey(key);
        }

        public void AddKeyWithDoubleClick()
        {
            var key = new TimelineKeyViewModel(Owner.MousePositionOnTimelineLane.X);
            AddKey(key);
        }

        public void AddKey(TimelineKeyViewModel vm)
        {
            foreach(var key in _Keys)
            {
                key.IsSelected = false;
            }
            _Keys.Add(vm);
        }

        public void DeleteSelectedKeys()
        {
            var removeKeys = _Keys.Where(arg => arg.IsSelected).ToArray();

            foreach(var removeKey in removeKeys)
            {
                removeKey.Dispose();
                _Keys.Remove(removeKey);
            }
        }
    }

    public class MainWindowViewModel : ViewModel
    {
        public IEnumerable<TrackItemViewModel> Tracks => _Tracks;
        ObservableCollection<TrackItemViewModel> _Tracks = new ObservableCollection<TrackItemViewModel>();

        public ViewModelCommand AddTrackCommand => _AddTrackCommand.Get(AddTrack);
        ViewModelCommandHandler _AddTrackCommand = new ViewModelCommandHandler();

        public ViewModelCommand RemoveTracksCommand => _RemoveTracksCommand.Get(RemoveTracks);
        ViewModelCommandHandler _RemoveTracksCommand = new ViewModelCommandHandler();

        public ViewModelCommand AddKeyCommand => _AddKeyCommand.Get(AddKey);
        ViewModelCommandHandler _AddKeyCommand = new ViewModelCommandHandler();

        public ListenerCommand<Point> BeginKeyMovingCommand => _BeginKeyMovingCommand.Get(BeginKeyMoving);
        ViewModelCommandHandler<Point> _BeginKeyMovingCommand = new ViewModelCommandHandler<Point>();

        public ListenerCommand<Point> KeyMovingCommand => _KeyMovingCommand.Get(KeyMoving);
        ViewModelCommandHandler<Point> _KeyMovingCommand = new ViewModelCommandHandler<Point>();

        public ListenerCommand<Point> EndKeyMovingCommand => _EndKeyMovingCommand.Get(EndKeyMoving);
        ViewModelCommandHandler<Point> _EndKeyMovingCommand = new ViewModelCommandHandler<Point>();

        public ViewModelCommand LaneClickedCommand => _LaneClickedCommand.Get(LaneClicked);
        ViewModelCommandHandler _LaneClickedCommand = new ViewModelCommandHandler();

        public ViewModelCommand DeleteOnTrackCommand => _DeleteOnTrackCommand.Get(DeleteOnTrack);
        ViewModelCommandHandler _DeleteOnTrackCommand = new ViewModelCommandHandler();

        public ViewModelCommand DeleteOnLaneCommand => _DeleteOnLaneCommand.Get(DeleteOnLane);
        ViewModelCommandHandler _DeleteOnLaneCommand = new ViewModelCommandHandler();

        public double CurrentValue
        {
            get => _CurrentValue;
            set => RaisePropertyChangedIfSet(ref _CurrentValue, value);
        }
        double _CurrentValue = 0;

        public Point MousePositionOnTimelineLane
        {
            get => _MousePositionOnTimelineLane;
            set => RaisePropertyChangedIfSet(ref _MousePositionOnTimelineLane, value);
        }
        Point _MousePositionOnTimelineLane;

        public Point ContextMenuOpeningPosition
        {
            get => _ContextMenuOpeningPosition;
            set => RaisePropertyChangedIfSet(ref _ContextMenuOpeningPosition, value);
        }
        Point _ContextMenuOpeningPosition;

        Point _CapturedBaseKeyPosition = new Point(0, 0);

        TimelineKeyViewModel[]? _SelectedKeyVMs = null;
        double[]? _SelectedKeyOffsetPlacements = null;

        public MainWindowViewModel()
        {
            var test1Lane = new TrackItemViewModel("test1", this);
            test1Lane.AddKey();
            test1Lane.Keys.ElementAt(0).IsSelected = true;

            _Tracks.Add(test1Lane);
            _Tracks.Add(new TrackItemViewModel("test2", this));
        }

        void AddTrack()
        {
            _Tracks.Add(new TrackItemViewModel($"test{_Tracks.Count}", this));
        }

        void RemoveTracks()
        {
            var removeTracks = _Tracks.Where(arg => arg.IsSelected).ToArray();

            foreach (var removeTrack in removeTracks)
            {
                _Tracks.Remove(removeTrack);
            }
        }

        public void AddKey()
        {
            var addKeyTracks = _Tracks.Where(arg => arg.IsSelected);

            foreach(var addKeyTrack in addKeyTracks)
            {
                var key = new TimelineKeyViewModel(ContextMenuOpeningPosition.X);
                addKeyTrack.AddKey(key);
            }
        }

        void BeginKeyMoving(Point pos)
        {
            _CapturedBaseKeyPosition = pos;
            _SelectedKeyVMs = _Tracks.SelectMany(arg => arg.Keys.Where(key => key.IsSelected)).ToArray();
            _SelectedKeyOffsetPlacements = _SelectedKeyVMs.Select(arg => arg.PlacementPosition).ToArray();
        }

        void KeyMoving(Point pos)
        {
            if(_SelectedKeyVMs == null || _SelectedKeyOffsetPlacements == null)
            {
                throw new InvalidProgramException();
            }

            var delta = pos.X - _CapturedBaseKeyPosition.X;
            for(int i=0; i<_SelectedKeyVMs.Length; i++)
            {
                var key = _SelectedKeyVMs[i];
                var offset = _SelectedKeyOffsetPlacements[i];
                key.PlacementPosition = offset + delta;
            }
        }

        void EndKeyMoving(Point pos)
        {
            _SelectedKeyVMs = null;
            _SelectedKeyOffsetPlacements = null;
        }

        void LaneClicked()
        {
            foreach(var key in _Tracks.SelectMany(arg => arg.Keys))
            {
                key.IsSelected = false;
            }
        }

        void DeleteOnTrack()
        {
            var removeTracks = _Tracks.Where(arg => arg.IsSelected).ToArray();
            foreach(var removeTrack in removeTracks)
            {
                removeTrack.Dispose();
                _Tracks.Remove(removeTrack);
            }
        }

        void DeleteOnLane()
        {
            foreach (var track in _Tracks)
            {
                track.DeleteSelectedKeys();
            }
        }
    }
}
