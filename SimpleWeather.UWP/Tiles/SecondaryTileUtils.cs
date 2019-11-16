using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.StartScreen;

namespace SimpleWeather.UWP.Tiles
{
    public static class SecondaryTileUtils
    {
        // Shared Settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static ApplicationDataContainer tileIdContainer = localSettings.CreateContainer("SecondaryTileIds", ApplicationDataCreateDisposition.Always);

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

        public static async Task UpdateTileId(string oldQuery, string newQuery)
        {
            string oldId = tileIdContainer.Values[oldQuery]?.ToString();

            if (oldId != null)
            {
                tileIdContainer.Values.Remove(oldQuery);

                if (!string.IsNullOrWhiteSpace(tileIdContainer.Values[newQuery]?.ToString()))
                {
                    if (SecondaryTile.Exists(tileIdContainer.Values[newQuery]?.ToString()))
                    {
                        await new SecondaryTile(tileIdContainer.Values[newQuery]?.ToString()).RequestDeleteAsync();
                    }
                }

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