using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.StartScreen;

namespace SimpleWeather.UWP.Helpers
{
    public static class SecondaryTileUtils
    {
        // Shared Settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer tileIdContainer = null;

        static SecondaryTileUtils()
        {
            // Container for secondary tile ids
            tileIdContainer =
                localSettings.CreateContainer("SecondaryTileIds", ApplicationDataCreateDisposition.Always);
        }

        public static String GetTileId(String query)
        {
            if (String.IsNullOrWhiteSpace(query))
            {
                return null;
            }
            else if (tileIdContainer.Values.ContainsKey(query))
            {
                return tileIdContainer.Values[query].ToString();
            }
            else
                return null;
        }

        public static String GetQueryFromId(String tileId)
        {
            var obj = tileIdContainer.Values.FirstOrDefault(q => tileId.Equals(q.Value));
            if (!String.IsNullOrWhiteSpace(obj.Key))
            {
                return obj.Key;
            }
            else
            {
                return null;
            }
        }

        public static async void UpdateTileId(String oldQuery, String newQuery)
        {
            String oldId = tileIdContainer.Values[oldQuery]?.ToString();

            if (oldId != null)
            {
                tileIdContainer.Values.Remove(oldQuery);

                if (!String.IsNullOrWhiteSpace(tileIdContainer.Values[newQuery]?.ToString()))
                {
                    if (SecondaryTile.Exists(tileIdContainer.Values[newQuery]?.ToString()))
                    {
                        await new SecondaryTile(tileIdContainer.Values[newQuery]?.ToString()).RequestDeleteAsync();
                    }
                }

                tileIdContainer.Values[newQuery] = oldId;
            }
        }

        public static void AddTileId(String query, String id)
        {
            tileIdContainer.Values[query] = id;
        }

        public static void RemoveTileId(String query)
        {
            if (tileIdContainer.Values.ContainsKey(query))
            {
                tileIdContainer.Values.Remove(query);
            }
        }

        public static bool Exists(String query)
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
