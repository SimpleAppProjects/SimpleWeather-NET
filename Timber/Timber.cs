namespace TimberLog
{
    public enum LoggerLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    /* Simple logging implementation based on Timber: https://github.com/JakeWharton/timber */

    public static class Timber
    {
        private static readonly Tree[] TREE_ARRAY_EMPTY = Array.Empty<Tree>();
        private static readonly IList<Tree> FOREST = new List<Tree>();
        private static volatile Tree[] forestAsArray = TREE_ARRAY_EMPTY;

        private static readonly Tree ROOT = new RootTree();

        public static void Log(LoggerLevel loggerLevel, string value, params object[] args)
        {
            ROOT.Log(loggerLevel, null, value, args);
        }

        public static void Log(LoggerLevel loggerLevel, Exception ex)
        {
            ROOT.Log(loggerLevel, ex, null, null);
        }

        public static void Log(LoggerLevel loggerLevel, Exception ex, string value)
        {
            ROOT.Log(loggerLevel, ex, value, null);
        }

        public static void Log(LoggerLevel loggerLevel, Exception ex, string format, params object[] args)
        {
            ROOT.Log(loggerLevel, ex, format, args);
        }

        public static void Plant(Tree tree)
        {
            if (tree is null)
            {
                throw new ArgumentNullException(nameof(tree));
            }
            if (tree == ROOT)
            {
                throw new ArgumentException("Cannot plant into oneself");
            }

            lock (FOREST)
            {
                FOREST.Add(tree);
                forestAsArray = FOREST.ToArray();
            }
        }

        public static void Uproot(Tree tree)
        {
            lock (FOREST)
            {
                if (!FOREST.Remove(tree))
                {
                    throw new ArgumentException("Cannot uproot tree which is not planted: " + tree);
                }
                forestAsArray = FOREST.ToArray();
            }
        }

        public static void UprootAll()
        {
            lock (FOREST)
            {
                FOREST.Clear();
                forestAsArray = TREE_ARRAY_EMPTY;
            }
        }

        public static IReadOnlyList<Tree> Forest
        {
            get
            {
                lock (FOREST)
                {
                    return FOREST.ToList();
                }
            }
        }

        public abstract class Tree
        {
            public void Log(LoggerLevel loggerLevel, string message, params object[] args)
            {
                PrepareLog(loggerLevel, null, message, args);
            }

            public void Log(LoggerLevel loggerLevel, Exception ex)
            {
                PrepareLog(loggerLevel, ex, null, null);
            }

            public void Log(LoggerLevel loggerLevel, Exception ex, string message)
            {
                PrepareLog(loggerLevel, ex, message, null);
            }

            public void Log(LoggerLevel loggerLevel, Exception ex, string message, params object[] args)
            {
                PrepareLog(loggerLevel, ex, message, args);
            }

            private void PrepareLog(LoggerLevel loggerLevel, Exception ex, string message, params object[] args)
            {
                if (message?.Length == 0)
                {
                    message = null;
                }
                if (message == null)
                {
                    if (ex == null)
                    {
                        return; // Swallow message if it's null and there's no throwable.
                    }
                    message = ex.ToString();
                }
                else
                {
                    if (args?.Length > 0)
                    {
                        message = String.Format(message, args);
                    }
                    if (ex != null)
                    {
                        message += Environment.NewLine + ex.ToString();
                    }
                }

                Log(loggerLevel, message, ex);
            }

            protected internal abstract void Log(LoggerLevel loggerLevel, string message, Exception exception);
        }

        private sealed class RootTree : Tree
        {
            protected internal override void Log(LoggerLevel loggerLevel, string message, Exception exception)
            {
                Tree[] forest = forestAsArray;
                foreach (Tree tree in forest)
                {
                    tree.Log(loggerLevel, message, exception);
                }
            }
        }

        public class DebugTree : Tree
        {
            protected internal override void Log(LoggerLevel loggerLevel, string message, Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(loggerLevel.ToString().ToUpper() + "|" + message);
            }
        }
    }
}