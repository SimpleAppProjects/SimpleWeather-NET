using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CacheCow.Common;

// HEAD: 0216b58a05b2363db210e992f6744833295986d1
namespace CacheCow.Client.FileCacheStore
{
    /// <summary>
    /// A simple 'cache-to-file' storage with persistanty over multiple runs.
    /// </summary>
    public class FileStore : ICacheStore
    {
        private readonly MessageContentHttpMessageSerializer _serializer = new MessageContentHttpMessageSerializer();

        /// <summary>
        /// The directory location of the cache
        /// </summary>
        private readonly string _cacheRoot;

        /// <summary>
        /// Minimum expiry of items. Default is 6 hours.
        /// Bear in mind, even expired items can be used if we do a cache validation request and get back 304
        /// </summary>
        public TimeSpan MinExpiry { get; set; }

        private static readonly List<string> ForbiddenDirectories =
            new List<string>()
            {
                "/",
                "",
                ".",
                ".."
            };

        /// <summary>
        /// Create a new Cachestore within the given directory. Responses will be saved in this directory.
        /// The directory should not be "/", ".", "" or null.
        /// Note that _all_ contents of this directory can be cleared.
        /// </summary>
        /// <param name="cacheRoot">The directory containing the cache</param>
        /// <exception cref="ArgumentException">When the passed directory is "/", ".", ".." or ""</exception>
        public FileStore(string cacheRoot)
        {
            if (cacheRoot is null || ForbiddenDirectories.Contains(cacheRoot))
            {
                throw new ArgumentException(
                    "The given caching directory is null or invalid. Do give an explicit caching directory, not empty, '/' or '.'. This will prevent accidents when cleaning the cache");
            }

            _cacheRoot = cacheRoot;
            if (!Directory.Exists(_cacheRoot))
            {
                Directory.CreateDirectory(cacheRoot);
            }
        }

        /// <inheritdoc />
        /// <exception cref="TimeoutException">Occurs if task times out trying to access file</exception>
        /// <exception cref="IOException">Occurs if there is an error accessing the file</exception>
        public async Task<HttpResponseMessage> GetValueAsync(CacheKey key)
        {
            if (!File.Exists(_pathFor(key)))
            {
                return null;
            }

            using var fs = await GetFile(_pathFor(key), FileAccess.Read);
            return await _serializer.DeserializeToResponseAsync(fs);
        }

        /// <inheritdoc />
        /// <exception cref="TimeoutException">Occurs if task times out trying to access file</exception>
        /// <exception cref="IOException">Occurs if there is an error accessing the file</exception>
        public async Task AddOrUpdateAsync(CacheKey key, HttpResponseMessage response)
        {
            using var fs = await GetFile(_pathFor(key), FileAccess.Write);
            await _serializer.SerializeAsync(response, fs);
        }

        /// <summary>
        /// Gets the file stream for the specified file path
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="access">The file access needed</param>
        /// <returns>FileStream for the file</returns>
        /// <exception cref="TimeoutException">Occurs if task times out trying to access file</exception>
        /// <exception cref="IOException">Occurs if there is an error accessing the file</exception>
        private static async Task<FileStream> GetFile(string filePath, FileAccess access)
        {
            int retry = 0;
            int delay = 250;

            while (retry < 5)
            {
                try
                {
                    switch (access)
                    {
                        case FileAccess.Read:
                            return File.OpenRead(filePath);
                        default:
                        case FileAccess.ReadWrite:
                            return File.Open(filePath, FileMode.OpenOrCreate, access);
                        case FileAccess.Write:
                            return File.OpenWrite(filePath);
                    }
                }
                catch (IOException e)
                {
                    // 0x80070020 - ERROR_SHARING_VIOLATION
                    if (e.Message.Contains("process cannot access") || e.HResult == 0x80070020)
                    {
                        await Task.Delay(delay);
                        retry++;
                        delay *= 2;
                    }
                    else
                    {
                        throw e;
                    }
                }
            }

            throw new TimeoutException("Could not access file after timeout and 5 retries: " + filePath);
        }

        /// <inheritdoc />
        public async Task<bool> TryRemoveAsync(CacheKey key)
        {
            if (!File.Exists(_pathFor(key)))
            {
                return false;
            }

            File.Delete(_pathFor(key));
            return true;
        }


        /// <inheritdoc />
        public async Task ClearAsync()
        {
            foreach (var f in Directory.GetFiles(_cacheRoot))
            {
                File.Delete(f);
            }
        }

        private string _pathFor(CacheKey key)
        {
            // Base64 might return "/" as character. This breaks files; so we replace the '/' with '!'
            return _cacheRoot + "/" + key.HashBase64.Replace('/', '!');
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to do here
        }

        /// <summary>
        /// Checks if the cache is empty
        /// </summary>
        /// <returns>True if no files are in the current cache</returns>
        public bool IsEmpty()
        {
            return Directory.GetFiles(_cacheRoot).Length == 0;
        }
    }
}