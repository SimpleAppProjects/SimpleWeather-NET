using CacheCow.Client.FileCacheStore;
using CacheCow.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.HttpClientExtensions
{
    public class RemoveHeaderDelagatingCacheStore : ICacheStore
    {
        private readonly ICacheStore _innerCacheStore;

        public RemoveHeaderDelagatingCacheStore(ICacheStore cacheStore)
        {
            _innerCacheStore = cacheStore;
        }

        public Task<HttpResponseMessage> GetValueAsync(CacheKey key)
        {
            return _innerCacheStore.GetValueAsync(key);
        }

        public Task AddOrUpdateAsync(CacheKey key, HttpResponseMessage response)
        {
            // Issue: https://github.com/aliostad/CacheCow/issues/213
            response.Headers.Server.Clear();

            // Handle invalid "Expires" header values
            if (response.Content.Headers.Contains(HttpHeaderNames.Expires))
            {
                if (response.Content.Headers.Expires == null)
                {
                    response.Content.Headers.Expires = DateTimeOffset.MinValue;
                }
            }
            
            return _innerCacheStore.AddOrUpdateAsync(key, response);
        }

        public Task<bool> TryRemoveAsync(CacheKey key)
        {
            return _innerCacheStore.TryRemoveAsync(key);
        }

        public Task ClearAsync()
        {
            return _innerCacheStore.ClearAsync();
        }

        public void Dispose()
        {
            _innerCacheStore.Dispose();
        }
    }
}
