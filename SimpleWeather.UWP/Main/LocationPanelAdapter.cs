using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.UWP.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SimpleWeather.UWP.Main
{
    internal class LocationPanelAdapter
    {
        private CollectionViewSource _ViewSource;
        private ObservableCollection<LocationPanelGroup> LocationPanelGroups;

        private ListViewBase ParentListView;
        private SnackbarManager SnackMgr;

        internal bool HasGPSPanel { get; private set; }
        internal bool HasSearchPanel { get; private set; }

        internal IEnumerable<LocationPanelViewModel> GetDataset()
        {
            var list = new List<LocationPanelViewModel>();
            foreach (var grp in LocationPanelGroups)
            {
                list.AddRange(grp.LocationPanels);
            }
            return list;
        }

        internal Collection<LocationPanelViewModel> GetDataset(LocationType locationType)
        {
            return LocationPanelGroups.Single(grp => grp.LocationType == locationType)?.LocationPanels;
        }

        internal CollectionViewSource ViewSource { get { return _ViewSource; } }

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

        internal LocationPanelAdapter(ListViewBase listViewBase)
        {
            if (listViewBase == null)
                throw new ArgumentNullException(nameof(listViewBase));

            ParentListView = listViewBase;
            ParentListView.Loaded += (s, e) =>
            {
                SnackMgr = new SnackbarManager(VisualTreeHelperExtensions.GetParent<Panel>(ParentListView));
            };

            _ViewSource = new CollectionViewSource()
            {
                IsSourceGrouped = true,
                ItemsPath = new PropertyPath("LocationPanels")
            };
            LocationPanelGroups = new ObservableCollection<LocationPanelGroup>()
            {
                new LocationPanelGroup(LocationType.GPS),
                new LocationPanelGroup(LocationType.Search),
            };
            foreach (var grp in LocationPanelGroups)
            {
                grp.LocationPanels.CollectionChanged += LocationPanels_CollectionChanged;
            }
            _ViewSource.Source = LocationPanelGroups;
        }

        private void LocationPanels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newItems = e.NewItems.Cast<LocationPanelViewModel>();

                if (!HasGPSPanel && GetDataset(LocationType.GPS)?.Count > 0)
                    HasGPSPanel = true;
                else if (!HasSearchPanel)
                    HasSearchPanel = true;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var items = e.NewItems?.Cast<LocationPanelViewModel>();
                if (items == null)
                    items = e.OldItems?.Cast<LocationPanelViewModel>();

                if (HasGPSPanel && GetDataset(LocationType.GPS)?.Count == 0)
                    HasGPSPanel = false;
            }

            ListChanged?.Invoke(sender, e);
        }

        internal LocationPanelViewModel GetGPSPanel()
        {
            if (HasGPSPanel)
                return LocationPanelGroups
                    .Single(grp => grp.LocationType == LocationType.GPS)
                    .LocationPanels.FirstOrDefault();
            return null;
        }

        internal LocationPanelViewModel GetFirstFavPanel()
        {
            return LocationPanelGroups
                .Single(grp => grp.LocationType == LocationType.Search)
                .LocationPanels.FirstOrDefault();
        }

        internal LocationData GetPanelData(int position)
        {
            if (position >= ItemCount || ItemCount == 0)
                return null;

            var panel = GetDataset()?.ElementAtOrDefault(position);
            if (panel == null) return null;
            return panel.LocationData;
        }

        internal void Add(LocationPanelViewModel item)
        {
            LocationPanelGroups
                .Single(grp => (int)grp.LocationType == item.LocationType)
                ?.LocationPanels.Add(item);
        }

        internal void Add(int index, LocationPanelViewModel item)
        {
            LocationPanelGroups
                .Single(grp => (int)grp.LocationType == item.LocationType)
                ?.LocationPanels.Insert(index, item);
        }

        internal bool Remove(LocationPanelViewModel item)
        {
            return (bool)LocationPanelGroups
                .Single(grp => (int)grp.LocationType == item.LocationType)
                ?.LocationPanels.Remove(item);
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
                group.LocationPanels.Clear();

            HasGPSPanel = false;
            HasSearchPanel = false;
        }

        internal void DeletePanel(LocationPanelViewModel view)
        {
            LocationData data = view.LocationData;

            Task.Run(async () =>
            {
                if (view != null)
                {
                    // Remove location from list
                    await Settings.DeleteLocation(data.query);

                    // Remove secondary tile if it exists
                    if (SecondaryTileUtils.Exists(data.query))
                    {
                        await new SecondaryTile(
                            SecondaryTileUtils.GetTileId(data.query)).RequestDeleteAsync();
                    }
                }
            });
        }

        internal Task RemovePanel(LocationPanelViewModel panel)
        {
            return ParentListView?.Dispatcher?.RunOnUIThread(() =>
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
                    SnackMgr.Show(Snackbar.MakeWarning(App.ResLoader.GetString("message_needfavorite"), SnackbarDuration.Short));
                    return;
                }

                // Show undo snackbar
                Snackbar snackbar = Snackbar.Make(App.ResLoader.GetString("message_locationremoved"), SnackbarDuration.Short);
                snackbar.SetAction(App.ResLoader.GetString("undo"), () =>
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

        internal Task BatchRemovePanels(IEnumerable<LocationPanelViewModel> panelsToDelete)
        {
            return ParentListView?.Dispatcher?.RunOnUIThread(() =>
            {
                if (!panelsToDelete.Any()) return;

                var panelPairs = new List<KeyValuePair<int, LocationPanelViewModel>>(panelsToDelete.Count());
                foreach (LocationPanelViewModel panel in panelsToDelete)
                {
                    int dataPosition = GetDataset((LocationType)panel.LocationType).IndexOf(panel);
                    panelPairs.Add(new KeyValuePair<int, LocationPanelViewModel>(dataPosition, panel));
                }

                // Create actions
                Action UndoAction = delegate
                {
                    foreach (var panelPair in panelPairs.OrderBy(p => p.Key))
                    {
                        var collection = GetDataset((LocationType)panelPair.Value.LocationType);

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
                    SnackMgr?.Show(Snackbar.MakeWarning(App.ResLoader.GetString("message_needfavorite"), SnackbarDuration.Short));
                    return;
                }

                // Show undo snackbar
                Snackbar snackbar = Snackbar.Make(App.ResLoader.GetString("message_locationremoved"), SnackbarDuration.Short);
                snackbar.SetAction(App.ResLoader.GetString("undo"), () =>
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
    {
        public LocationPanelGroup(LocationType locationType)
        {
            this.LocationType = locationType;
            this.LocationPanels = new ObservableCollection<LocationPanelViewModel>();
        }

        public LocationType LocationType { get; set; }
        public ObservableCollection<LocationPanelViewModel> LocationPanels { get; private set; }
    }
}