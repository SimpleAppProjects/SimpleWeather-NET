#if WINDOWS
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.StartScreen;

namespace SimpleWeather.Uno.Tiles
{
    public static class SecondaryTileUtils
    {
        // Shared Settings
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static readonly ApplicationDataContainer tileIdContainer = localSettings.CreateContainer("SecondaryTileIds", ApplicationDataCreateDisposition.Always);

        public static string GetTileId(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }
            else if (tileIdContainer.Values.ContainsKey(query))
            {
                return tileIdContainer.Values[query].ToString();
            }
            else
            {
                return null;
            }
        }

        public static string GetQueryFromId(string tileId)
        {
            var obj = tileIdContainer.Values.FirstOrDefault(q => tileId.Equals(q.Value));
            if (!string.IsNullOrWhiteSpace(obj.Key))
            {
                return obj.Key;
            }
            else
            {
                return null;
            }
        }

        public static void UpdateTileId(string oldQuery, string newQuery)
        {
            if (tileIdContainer.Values.ContainsKey(oldQuery))
            {
                string oldId = tileIdContainer.Values[oldQuery]?.ToString();

                tileIdContainer.Values.Remove(oldQuery);

                tileIdContainer.Values[newQuery] = oldId;
            }
        }

        public static void AddTileId(string query, string id)
        {
            tileIdContainer.Values[query] = id;
        }

        public static void RemoveTileId(string query)
        {
            if (tileIdContainer.Values.ContainsKey(query))
            {
                tileIdContainer.Values.Remove(query);
            }
        }

        public static bool Exists(string query)
        {
            var id = GetTileId(query);

            if (id == null)
            {
                return false;
            }
            else
            {
                return SecondaryTile.Exists(id);
            }
        }
    }
}
#endif