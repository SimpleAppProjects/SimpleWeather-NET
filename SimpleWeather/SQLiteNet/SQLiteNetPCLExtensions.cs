using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.SQLiteNet
{
    public static class SQLiteNetPCLExtensions
    {
        const int queryLimit = 990; //Make room for extra keys added by the code

        /// <summary>
        /// Deletes all the objects with IDs not equal to the passed parameters from the database.
        /// Relationships are not taken into account in this method
        /// </summary>
        /// <param name="conn">SQLite Net connection object</param>
        /// <param name="primaryKeyValues">Primary keys of the objects not to be deleted from the database</param>
        /// <typeparam name="T">The Entity type, it should match de database entity type</typeparam>
        public static void DeleteAllIdsNotIn<T>(this SQLiteConnection conn, IEnumerable<object> primaryKeyValues)
        {
            var type = typeof(T);
            var primaryKeyProperty = type.GetPrimaryKey();

            conn.DeleteAllIdsNotIn(primaryKeyValues.ToArray(), type.GetTableName(), primaryKeyProperty.GetColumnName());
        }

        public static void DeleteAllIdsNotIn<T>(this SQLiteConnection conn, String primaryKeyName, IEnumerable<object> primaryKeyValues)
        {
            var type = typeof(T);

            conn.DeleteAllIdsNotIn(primaryKeyValues.ToArray(), type.GetTableName(), primaryKeyName);
        }

        /// <summary>
        /// Deletes all the objects with IDs not equal to the passed parameters from the database.
        /// Relationships are not taken into account in this method
        /// </summary>
        /// <param name="conn">SQLite Net connection object</param>
        /// <param name="primaryKeyValues">Primary keys of the objects not to be deleted from the database</param>
        /// <typeparam name="T">The Entity type, it should match de database entity type</typeparam>
        public static Task DeleteAllIdsNotInAsync<T>(this SQLiteAsyncConnection conn, IEnumerable<object> primaryKeyValues)
        {
            return Task.Run(() =>
            {
                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(conn);
                using (connectionWithLock.Lock())
                {
                    connectionWithLock.DeleteAllIdsNotIn<T>(primaryKeyValues);
                }
            });
        }

        public static Task DeleteAllIdsNotInAsync<T>(this SQLiteAsyncConnection conn, String primaryKeyName, IEnumerable<object> primaryKeyValues)
        {
            return Task.Run(() =>
            {
                var connectionWithLock = SqliteAsyncConnectionWrapper.Lock(conn);
                using (connectionWithLock.Lock())
                {
                    connectionWithLock.DeleteAllIdsNotIn<T>(primaryKeyName, primaryKeyValues);
                }
            });
        }

        private static void DeleteAllIdsNotIn(this SQLiteConnection conn, object[] primaryKeyValues, string entityName, string primaryKeyName)
        {
            if (primaryKeyValues == null || primaryKeyValues.Length == 0)
                return;

            if (primaryKeyValues.Length <= queryLimit)
            {
                var placeholdersString = string.Join(",", Enumerable.Repeat("?", primaryKeyValues.Length));
                var deleteQuery = string.Format("delete from [{0}] where [{1}] not in ({2})", entityName, primaryKeyName, placeholdersString);

                conn.Execute(deleteQuery, primaryKeyValues);
            }
            else
            {
                foreach (var primaryKeys in Split(primaryKeyValues.ToList(), queryLimit))
                {
                    conn.DeleteAllIdsNotIn(primaryKeys.ToArray(), entityName, primaryKeyName);
                }
            }
        }

        private static List<List<T>> Split<T>(List<T> items, int sliceSize = 30)
        {
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < items.Count; i += sliceSize)
                list.Add(items.GetRange(i, Math.Min(sliceSize, items.Count - i)));
            return list;
        }
    }
}