namespace SimpleWeather.Maui.IncrementalLoadingCollection
{
    public partial interface ISupportIncrementalLoading
    {
        bool HasMoreItems { get; }
        Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count);
    }
}
