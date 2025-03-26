using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_Features : Page
    {
        private IDictionary<string, Feature> OrderableFeaturesMap;

        private CancellationTokenSource reorderToken;

        public Settings_Features()
        {
            this.InitializeComponent();

            this.Loaded += (s, e) =>
            {
                OrderableFeaturesMap = OrderableFeatures.ToDictionary(f => f.Key);

                var features = FeatureSettings.GetFeatureOrder()?.Intersect(OrderableFeaturesMap.Keys)?.Select(key =>
                {
                    return OrderableFeaturesMap[key];
                });

                FeatureListView.ItemsSource = new ObservableCollection<Feature>(features ?? OrderableFeatures).Apply(it =>
                {
                    it.CollectionChanged += Features_CollectionChanged;
                });
            };
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (FeatureListView.ItemsSource is ObservableCollection<Feature> c)
            {
                c.CollectionChanged -= Features_CollectionChanged;
            }

            // Reset
            FeatureListView.ItemsSource = new ObservableCollection<Feature>(OrderableFeatures).Apply(it =>
            {
                it.CollectionChanged += Features_CollectionChanged;
            });

            FeatureSettings.SetFeatureOrder(null);

            // Re-enable all features
            OrderableFeatures.Union(NonOrderableFeatures).ForEach(f =>
            {
                f.IsEnabled = true;
            });
        }

        private void Features_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Move || e.Action == NotifyCollectionChangedAction.Add)
            {
                reorderToken?.Cancel();

                var orderableFeatures = (FeatureListView.ItemsSource as IEnumerable<Feature>)?.ToList();

                if (orderableFeatures is not null)
                {
                    if (reorderToken?.TryReset() != true)
                    {
                        reorderToken = new CancellationTokenSource();
                    }

                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await Task.Delay(1000);

                        if (!reorderToken.IsCancellationRequested)
                        {
                            var keys = orderableFeatures.Select(f => f.Key);
                            FeatureSettings.SetFeatureOrder(keys);
                        }
                    });
                }
            }
        }

        private void Item_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Feature f)
            {
                f.IsEnabled = !f.IsEnabled;
            }
        }
    }

    public class FeatureList : List<Feature> { }

    public class Feature : DependencyObject
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Feature), new PropertyMetadata(default));

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(string), typeof(Feature), new PropertyMetadata(default));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(Feature), new PropertyMetadata(true));

        public bool CanReorder
        {
            get { return (bool)GetValue(CanReorderProperty); }
            set { SetValue(CanReorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanReorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanReorderProperty =
            DependencyProperty.Register("CanReorder", typeof(bool), typeof(Feature), new PropertyMetadata(true));
    }
}
