using SimpleWeather.Helpers;
using SimpleWeather.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SimpleWeather.Preferences
{
    internal class FilePropertySet : IPropertySet
    {
        private const string UWPFileName = ".UWPAppSettings";
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        private readonly string _folderPath;
        private readonly string _filePath;

        public FilePropertySet(string prefix = "")
        {
            _folderPath = ApplicationDataHelper.GetLocalFolderPath();
            _filePath = Path.Combine(_folderPath, prefix + UWPFileName);

            ReadFromFile();
        }

        public object? this[string key]
        {
            get
            {
                if (_values.TryGetValue(key, out var value))
                {
                    return DataTypeSerializer.Deserialize(value);
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    _values[key] = DataTypeSerializer.Serialize(value);
                }
                else
                {
                    Remove(key);
                }

                WriteToFile();
            }
        }

        private void ReadFromFile()
        {
            try
            {

                if (File.Exists(_filePath))
                {
                    using (var reader = new BinaryReader(File.OpenRead(_filePath)))
                    {
                        var count = reader.ReadInt32();

#if DEBUG
                        Logger.WriteLine(LoggerLevel.Debug, $"FilePropertySet: Reading {count} settings values");
#endif

                        for (int i = 0; i < count; i++)
                        {
                            var key = reader.ReadString();
                            var value = reader.ReadString();

                            _values[key] = value;
                        }
                    }
                }
                else
                {
#if DEBUG
                    Logger.WriteLine(LoggerLevel.Debug, $"FilePropertySet: File {_filePath} does not exist, skipping reading settings");
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Logger.WriteLine(LoggerLevel.Debug, $"FilePropertySet: Failed to read settings from {_filePath}", e);
#endif
            }
        }

        private void WriteToFile()
        {
            try
            {
                Directory.CreateDirectory(_folderPath);

#if DEBUG
                Logger.WriteLine(LoggerLevel.Debug, $"FilePropertySet: Writing {_values.Count} settings to {_filePath}");
#endif

                using (var writer = new BinaryWriter(File.OpenWrite(_filePath)))
                {
                    writer.Write(_values.Count);

                    foreach (var pair in _values)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value ?? "");
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Logger.WriteLine(LoggerLevel.Debug, $"FilePropertySet: Failed to write settings to {_filePath}", e);
#endif
            }
        }

        public ICollection<string> Keys
            => _values.Keys;

        public ICollection<object> Values
            => _values.Values.Select(DataTypeSerializer.Deserialize).ToList();

        public int Count
            => _values.Count;

        public bool IsReadOnly => false;

#pragma warning disable CS0067
        public event MapChangedEventHandler<string, object>? MapChanged;
#pragma warning restore CS0067

        public void Add(string key, object value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("An item with the same key has already been added.");
            }
            if (value != null)
            {
                _values.Add(key, DataTypeSerializer.Serialize(value));
                WriteToFile();
            }
        }

        public void Add(KeyValuePair<string, object> item)
            => Add(item.Key, item.Value);

        public void Clear()
        {
            _values.Clear();
            WriteToFile();
        }

        public bool Contains(KeyValuePair<string, object> item)
            => throw new NotSupportedException();

        public bool ContainsKey(string key)
            => _values.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            => throw new NotSupportedException();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => _values.Select(v => new KeyValuePair<string, object>(v.Key, v.Value)).GetEnumerator();

        public bool Remove(string key)
        {
            var ret = _values.Remove(key);

            WriteToFile();

            return ret;
        }

        public bool Remove(KeyValuePair<string, object> item) => Remove(item.Key);

        public bool TryGetValue(string key, out object? value)
        {
            if (_values.TryGetValue(key, out var innervalue))
            {
                value = DataTypeSerializer.Deserialize(innervalue);
                return true;
            }

            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class DataTypeSerializer
    {
        public readonly static Type[] SupportedTypes = new[]
        {
            typeof(bool),
            typeof(byte),
            typeof(char),
            typeof(char),
            typeof(DateTimeOffset),
            typeof(double),
            typeof(Guid),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(object),
            typeof(Point),
            typeof(Rect),
            typeof(float),
            typeof(Size),
            typeof(string),
            typeof(TimeSpan),
            typeof(byte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(Uri),
        };

        public static object Deserialize(string value)
        {
            var index = value?.IndexOf(':') ?? -1;

            if (index != -1)
            {
                string typeName = value.Substring(0, index);
                var dataType = Type.GetType(typeName) ?? Type.GetType(typeName + ", " + typeof(Point).GetTypeInfo().Assembly.FullName);
                var valueField = value.Substring(index + 1);

                if (dataType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(valueField, CultureInfo.InvariantCulture);
                }
                else if (dataType == typeof(Guid))
                {
                    return Guid.Parse(valueField);
                }
                else if (dataType == typeof(TimeSpan))
                {
                    return TimeSpan.Parse(valueField, CultureInfo.InvariantCulture);
                }
                else
                {
                    return Convert.ChangeType(valueField, dataType, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                return null;
            }
        }

        public static string Serialize(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueAsString = Convert.ToString(value, CultureInfo.InvariantCulture);

            return value.GetType().FullName + ":" + valueAsString;
        }
    }
}
