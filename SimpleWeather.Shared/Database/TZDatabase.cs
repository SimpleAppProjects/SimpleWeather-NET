using SimpleWeather.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace SimpleWeather.Database
{
    public partial class TZDatabase : BaseDatabase
    {
        private const string DatabaseFileName = "tzdb.db";
        private const int VERSION = 6;

        private static readonly Lazy<TZDatabase> lazyInitializer = new(() =>
        {
            return new TZDatabase();
        });

        public static TZDatabase Instance => lazyInitializer.Value;
        private static bool initialize = false;

        private readonly SQLiteAsyncConnection _database = new(GetDatabasePath(), SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);

        public SQLiteAsyncConnection Connection => _database;

        private TZDatabase() : base() { }

        protected override void Initialize()
        {
            base.Initialize();

            if (!initialize)
            {
                var conn = _database.GetConnection();
                using var _lock = conn.Lock();

                // Set database settings
                conn.BusyTimeout = TimeSpan.FromSeconds(5);
                conn.EnableWriteAheadLogging();

                // Create tables
                CreateDatabase(conn);

                UpdateDatabaseIfNeeded(conn);

                _lock.Dispose();
            }

            initialize = true;
        }

        protected override void CreateDatabase(SQLiteConnection conn)
        {
            conn.CreateTable<TZDB.TZDB>();
        }

        protected override void DestroyDatabase(SQLiteConnection conn)
        {
            conn.DropTable<TZDB.TZDB>();
        }

        protected override void UpdateDatabaseIfNeeded(SQLiteConnection conn)
        {
            int currentDBVersion = conn.GetDatabaseVersion();

            if (currentDBVersion < VERSION)
            {
                try
                {
                    AnalyticsLogger.LogEvent("TZDatabase: Migration started",
                        new Dictionary<string, string>()
                        {
                                { "Version", VERSION.ToInvariantString() },
                                { "CurrentDBVersion", currentDBVersion.ToInvariantString() }
                        });

                    switch (currentDBVersion)
                    {
                        case 0:
                            goto case 1;
                        case 1:
                            goto case 2;
                        case 2:
                            goto case 3;
                        case 3:
                            goto case 4;
                        case 4:
                            goto case 5;
                        case 5:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    AnalyticsLogger.LogEvent("TZDatabase: Migration failed",
                        new Dictionary<string, string>()
                        {
                                { "Version", VERSION.ToInvariantString() },
                                { "CurrentDBVersion", currentDBVersion.ToInvariantString() }
                        });

                    // Allow recovery if  migration fails since weatherdata is expendable
                    Logger.WriteLine(LoggerLevel.Error, e, "TZDatabase: Migration v{0} -> v{1} failed; Purging database!!", currentDBVersion, VERSION);

                    DestroyDatabase(conn);
                    CreateDatabase(conn);
                }
                finally
                {
                    conn.SetDatabaseToVersion(VERSION);
                }
            }
        }
    }
}
