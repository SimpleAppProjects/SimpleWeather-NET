namespace SimpleWeather.Maui.IncrementalLoadingCollection
{
    public partial interface ISupportIncrementalLoading
    {
        bool HasMoreItems { get; }
        bool IsLoading { get; }
        Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count);
    }
}
