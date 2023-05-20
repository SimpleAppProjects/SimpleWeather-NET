using CommunityToolkit.Mvvm.DependencyInjection;
#if WINDOWS
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endif
using SimpleWeather.LocationData;
using SimpleWeather.NET.Controls;
#if WINDOWS
using SimpleWeather.NET.Helpers;
using SimpleWeather.NET.Tiles;
using SimpleWeather.NET.Widgets;
#else
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Controls;
#endif
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if WINDOWS
using Windows.UI.StartScreen;
#endif
using ResStrings = SimpleWeather.Resources.Strings.Resources;

#if WINDOWS
namespace SimpleWeather.NET.Main
#else
namespace SimpleWeather.Maui.Main
#endif
{
    internal class LocationPanelAdapter
    {
#if WINDOWS
        private CollectionViewSource _ViewSource;
        private ObservableCollection<LocationPanelGroup> LocationPanelGroups;
#else
        internal ObservableCollection<LocationPanelGroup> LocationPanelGroups;
#endif

#if WINDOWS
        private ListViewBase ParentListView;
#else
        private CollectionView ParentListView;
#endif
        private SnackbarManager SnackMgr;

        internal bool HasGPSPanel { get; private set; }
        internal bool HasSearchPanel { get; private set; }

        internal IEnumerable<LocationPanelUiModel> GetDataset()
        {
            var list = new List<LocationPanelUiModel>();
            foreach (var grp in LocationPanelGroups)
            {
                list.AddRange(grp.LocationPanels);
            }
            return list;
        }

        internal Collection<LocationPanelUiModel> GetDataset(LocationType locationType)
        {
            return LocationPanelGroups.SingleOrDefault(grp => grp.LocationType == locationType)?.LocationPanels;
        }

#if WINDOWS
        internal CollectionViewSource ViewSource { get { return _ViewSource; } }
#endif

        internal event NotifyCollectionChangedEventHandler ListChanged;

        internal int ItemCount
        {
            get
            {
                int count = 0;
                foreach (var grp in LocationPanelGroups)
                {
                    count += grp.LocationPanels.Count;
                }
                return count;
            }
        }

        internal int FavoritesCount
        {
            get
            {
                int size = ItemCount;

                if (HasGPSPanel)
                    size--;

                return size;
            }
        }

#if WINDOWS
        internal LocationPanelAdapter(ListViewBase listViewBase)
#else
        internal LocationPanelAdapter(CollectionView listViewBase, IView SnackbarContainer = null)
#endif
        {
            if (listViewBase == null)
                throw new ArgumentNullException(nameof(listViewBase));

            ParentListView = listViewBase;
            ParentListView.Loaded += (s, e) =>
            {
#if WINDOWS
                SnackMgr = new SnackbarManager(VisualTreeHelperExtensions.GetParent<Panel>(ParentListView));
#else
                SnackMgr = new SnackbarManager(SnackbarContainer ?? ParentListView);
#endif
            };

#if WINDOWS
            _ViewSource = new CollectionViewSource()
            {
                IsSourceGrouped = true,
                ItemsPath = new PropertyPath("LocationPanels")
            };
#endif
            LocationPanelGroups = new ObservableCollection<LocationPanelGroup>()
            {
#if WINDOWS
                new LocationPanelGroup(LocationType.GPS),
                new LocationPanelGroup(LocationType.Search),
#endif
            };
            foreach (var grp in LocationPanelGroups)
            {
                grp.LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;
            }
#if WINDOWS
            _ViewSource.Source = LocationPanelGroups;
#endif
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newItems = e.NewItems.Cast<LocationPanelUiModel>();

                if (!HasGPSPanel && GetDataset(LocationType.GPS)?.Count > 0)
                    HasGPSPanel = true;
                else if (!HasSearchPanel)
                    HasSearchPanel = true;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var items = e.NewItems?.Cast<LocationPanelUiModel>();
                if (items == null)
                    items = e.OldItems?.Cast<LocationPanelUiModel>();

                if (HasGPSPanel && GetDataset(LocationType.GPS)?.Count == 0)
                    HasGPSPanel = false;
            }

            ListChanged?.Invoke(sender, e);
        }

        internal LocationPanelUiModel GetGPSPanel()
        {
            if (HasGPSPanel)
                return LocationPanelGroups
                    .SingleOrDefault(grp => grp.LocationType == LocationType.GPS)
                    .LocationPanels.FirstOrDefault();
            return null;
        }

        internal LocationPanelUiModel GetFirstFavPanel()
        {
            return LocationPanelGroups
                .SingleOrDefault(grp => grp.LocationType == LocationType.Search)
                .LocationPanels.FirstOrDefault();
        }

        internal LocationData.LocationData GetPanelData(int position)
        {
            if (position >= ItemCount || ItemCount == 0)
                return null;

            var panel = GetDataset()?.ElementAtOrDefault(position);
            if (panel == null) return null;
            return panel.LocationData;
        }

        internal void Add(LocationPanelUiModel item)
        {
#if WINDOWS
            LocationPanelGroups
                .Single(grp => (int)grp.LocationType == item.LocationType)
                ?.LocationPanels.Add(item);
#else
            var group = LocationPanelGroups.SingleOrDefault(grp => (int)grp.LocationType == item.LocationType);
            group ??= CreateLocationPanelGroup((LocationType)item.LocationType);
            group.LocationPanels.Add(item);
#endif
        }

        internal void Add(int index, LocationPanelUiModel item)
        {
#if WINDOWS
            LocationPanelGroups
                .Single(grp => (int)grp.LocationType == item.LocationType)
                ?.LocationPanels.Insert(index, item);
#else
            var group = LocationPanelGroups.SingleOrDefault(grp => (int)grp.LocationType == item.LocationType);
            group ??= CreateLocationPanelGroup((LocationType)item.LocationType);
            group.LocationPanels.Insert(index, item);
#endif
        }

        private LocationPanelGroup CreateLocationPanelGroup(LocationType locationType)
        {
            LocationPanelGroup group;

            if (locationType == LocationType.GPS)
                LocationPanelGroups.Insert(0, group = new LocationPanelGroup(locationType));
            else
                LocationPanelGroups.Add(group = new LocationPanelGroup(locationType));

            group.LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;

            return group;
        }

        internal void AddAll(IEnumerable<LocationPanelUiModel> items)
        {
            items?.ForEach(item => Add(item));
        }

        internal void ReplaceAll(IEnumerable<LocationPanelUiModel> items)
        {
            RemoveAll();
            AddAll(items);
        }

        internal bool Remove(LocationPanelUiModel item)
        {
#if WINDOWS
            return (bool)LocationPanelGroups
                .Single(grp => (int)grp.LocationType == item.LocationType)
                ?.LocationPanels.Remove(item);
#else
            var group = LocationPanelGroups.SingleOrDefault(grp => (int)grp.LocationType == item.LocationType);

            if (group != null)
            {
                try
                {
                    bool result = false;

                    try
                    {
                        result = group.Remove(item);
                    } catch { }

                    if (group.Count == 0)
                    {
                        group.CollectionChanged -= LocationPanels_CollectionChanged;
                        LocationPanelGroups.Remove(group);
                    }

                    return result;
                }
                catch { }
            }

            return false;
#endif
        }

        internal void RemoveGPSPanel()
        {
            var gpsPanel = GetGPSPanel();
            if (gpsPanel != null)
                Remove(gpsPanel);
        }

        internal void RemoveAll()
        {
            foreach (var group in LocationPanelGroups)
            {
#if WINDOWS
                group.LocationPanels.Clear();
#else
                try
                {
                    if (group.Count > 0)
                        group.Clear();
                }
                catch { }
#endif
            }

#if !WINDOWS
            LocationPanelGroups.Clear();
#endif

            HasGPSPanel = false;
            HasSearchPanel = false;
        }

        internal void DeletePanel(LocationPanelUiModel view)
        {
            var data = view.LocationData;

            Task.Run(async () =>
            {
                if (view != null)
                {
                    // Remove location from list
                    var SettingsManager = Ioc.Default.GetService<SettingsManager>();
                    await SettingsManager.DeleteLocation(data.query);

#if WINDOWS
                    // Remove secondary tile if it exists
                    if (SecondaryTileUtils.Exists(data.query))
                    {
                        await new SecondaryTile(
                            SecondaryTileUtils.GetTileId(data.query)).RequestDeleteAsync();
                    }
                    if (WidgetUtils.Exists(data.query))
                    {
                        WidgetUtils.GetWidgetIds(data.query).ForEach(id =>
                        {
                            WidgetUtils.DeleteWidget(id);
                        });
                    }
#endif
                }
            });
        }

        internal Task RemovePanel(LocationPanelUiModel panel)
        {
#if WINDOWS
            return ParentListView?.DispatcherQueue?.EnqueueAsync(() =>
#else
            return ParentListView?.Dispatcher?.DispatchAsync(() =>
#endif
            {
                int dataPosition = GetDataset((LocationType)panel.LocationType).IndexOf(panel);

                // Create actions
                Action UndoAction = delegate
                {
                    var collection = GetDataset((LocationType)panel.LocationType);

                    if (!collection.Contains(panel))
                    {
                        if (dataPosition >= collection.Count)
                        {
                            Add(panel);
                        }
                        else
                        {
                            Add(dataPosition, panel);
                        }
                    }
                };

                Remove(panel);

                if (FavoritesCount <= 0)
                {
                    UndoAction.Invoke();
                    SnackMgr.Show(Snackbar.MakeWarning(ResStrings.message_needfavorite, SnackbarDuration.Short));
                    return;
                }

                // Show undo snackbar
                Snackbar snackbar = Snackbar.Make(ResStrings.message_locationremoved, SnackbarDuration.Short);
                snackbar.SetAction(ResStrings.undo, () =>
                {
                    //panel = null;
                    UndoAction.Invoke();
                });
                snackbar.Dismissed = (sender, @event) =>
                {
                    if (@event != SnackbarDismissEvent.Action)
                    {
                        DeletePanel(panel);
                    }
                };
                SnackMgr.Show(snackbar);
            });
        }

        internal Task BatchRemovePanels(IEnumerable<LocationPanelUiModel> panelsToDelete)
        {
#if WINDOWS
            return ParentListView?.DispatcherQueue?.EnqueueAsync(() =>
#else
            return ParentListView?.Dispatcher?.DispatchAsync(() =>
#endif
            {
                if (!panelsToDelete.Any()) return;

                var panelPairs = new List<KeyValuePair<int, LocationPanelUiModel>>(panelsToDelete.Count());
                foreach (LocationPanelUiModel panel in panelsToDelete)
                {
                    int dataPosition = GetDataset((LocationType)panel.LocationType).IndexOf(panel);
                    panelPairs.Add(new KeyValuePair<int, LocationPanelUiModel>(dataPosition, panel));
                }

                // Create actions
                Action UndoAction = delegate
                {
                    foreach (var panelPair in panelPairs.OrderBy(p => p.Key))
                    {
                        var collection = GetDataset((LocationType)panelPair.Value.LocationType);
#if !WINDOWS
                        collection ??= CreateLocationPanelGroup((LocationType)panelPair.Value.LocationType);
#endif

                        if (!collection.Contains(panelPair.Value))
                        {
                            if (panelPair.Key >= collection.Count)
                            {
                                Add(panelPair.Value);
                            }
                            else
                            {
                                Add(panelPair.Key, panelPair.Value);
                            }
                        }
                    }
                };

                foreach (var panelPair in panelPairs)
                {
                    Remove(panelPair.Value);
                }

                if (FavoritesCount <= 0)
                {
                    UndoAction.Invoke();
                    SnackMgr?.Show(Snackbar.MakeWarning(ResStrings.message_needfavorite, SnackbarDuration.Short));
                    return;
                }

                // Show undo snackbar
                Snackbar snackbar = Snackbar.Make(ResStrings.message_locationremoved, SnackbarDuration.Short);
                snackbar.SetAction(ResStrings.undo, () =>
                {
                    //panel = null;
                    UndoAction.Invoke();
                });
                snackbar.Dismissed = (sender, @event) =>
                {
                    if (@event != SnackbarDismissEvent.Action)
                    {
                        foreach (var panelPair in panelPairs)
                        {
                            DeletePanel(panelPair.Value);
                        }
                    }
                };
                SnackMgr?.Show(snackbar);
            });
        }
    }

    internal class LocationPanelGroup
#if !WINDOWS
        : ObservableCollection<LocationPanelUiModel>
#endif
    {
        public LocationPanelGroup(LocationType locationType)
        {
            this.LocationType = locationType;
#if WINDOWS
            this.LocationPanels = new ObservableCollection<LocationPanelUiModel>();
#endif
        }

        public LocationType LocationType { get; set; }
#if WINDOWS
        public ObservableCollection<LocationPanelUiModel> LocationPanels { get; private set; }
#else
        public ObservableCollection<LocationPanelUiModel> LocationPanels { get => this; }
#endif
    }
}