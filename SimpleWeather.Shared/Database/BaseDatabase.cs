using SQLite;
using SQLiteNetExtensions.Extensions.TextBlob;

namespace SimpleWeather.Database
{
    public abstract class BaseDatabase
    {
        public BaseDatabase()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (TextBlobOperations.GetTextSerializer() is not DBTextBlobSerializer)
            {
                TextBlobOperations.SetTextSerializer(new DBTextBlobSerializer());
            }
        }

        protected abstract void CreateDatabase(SQLiteConnection conn);
        protected abstract void DestroyDatabase(SQLiteConnection conn);
        protected abstract void UpdateDatabaseIfNeeded(SQLiteConnection conn);
    }
}
