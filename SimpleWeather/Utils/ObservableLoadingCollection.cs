using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.Utils
{
    public interface IObservableLoadingCollection : ISupportIncrementalLoading
    {
        bool IsLoading { get; set; }
    }

    public abstract class ObservableLoadingCollection<T> : ObservableCollection<T>, IObservableLoadingCollection
    {
        protected bool _isLoading;
        protected bool _hasMoreItems;
        protected bool _refreshOnLoad;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                }
            }
        }

        public bool HasMoreItems
        {
            get { return _hasMoreItems; }
            set
            {
                if (value != _hasMoreItems)
                {
                    _hasMoreItems = value;
                }
            }
        }

        public Task RefreshAsync()
        {
            if (IsLoading)
            {
                _refreshOnLoad = true;
            }
            else
            {
                Clear();
                HasMoreItems = true;

                return LoadMoreItemsAsync(1).AsTask();
            }

            return Task.CompletedTask;
        }

        public abstract IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count);
    }
}