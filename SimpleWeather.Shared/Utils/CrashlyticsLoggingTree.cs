#if NET8_0_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Firebase.Crashlytics;
using Foundation;
using Plugin.Firebase.Crashlytics;
using TimberLog;
using FirebaseCrashlytics = Firebase.Crashlytics.Crashlytics;

namespace SimpleWeather.Utils
{
    public class CrashlyticsLoggingTree : Timber.Tree
    {
        private FirebaseCrashlytics crashlytics = FirebaseCrashlytics.SharedInstance;
        private readonly NSObject KEY_PRIORITY = NSString.FromObject("priority");
        private readonly NSObject KEY_TAG = NSString.FromObject("tag");
        private readonly NSObject KEY_MESSAGE = NSString.FromObject("message");

        protected override bool IsLoggable(string category, TimberLog.LoggerLevel loggerLevel)
        {
            return loggerLevel > TimberLog.LoggerLevel.Debug;
        }

        protected override void Log(TimberLog.LoggerLevel loggerLevel, string tag, string message, Exception exception)
        {
            try
            {
                var priorityTAG = loggerLevel switch
                {
                    TimberLog.LoggerLevel.Debug => "DEBUG",
                    TimberLog.LoggerLevel.Info => "INFO",
                    TimberLog.LoggerLevel.Warn => "WARN",
                    TimberLog.LoggerLevel.Error => "ERROR",
                    TimberLog.LoggerLevel.Fatal => "FATAL",
                    _ => "DEBUG",
                };

                crashlytics ??= FirebaseCrashlytics.SharedInstance;

                crashlytics.SetCustomValue(KEY_PRIORITY, priorityTAG);
                if (tag != null) crashlytics.SetCustomValue(KEY_TAG, tag);
                crashlytics.SetCustomValue(KEY_MESSAGE, message);

                if (tag != null)
                {
                    crashlytics.Log($"{priorityTAG} | {tag}: {message}");
                }
                else
                {
                    crashlytics.Log($"{priorityTAG} | {message}");
                }

                if (exception != null)
                {
                    crashlytics.RecordExceptionModel(exception.CreateExceptionModel());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"CrashlyticsLoggingTree: Error while logging : {e}");
            }
        }
    }
}

// source: https://github.com/TobiasBuchholz/Plugin.Firebase
namespace Plugin.Firebase.Crashlytics
{
    public static class StackTraceParser
    {
        private static readonly Regex _regex =
 new(@"^\s*at (?<className>.+)\.(?<methodName>.+\(.*\))( in (?<fileName>.+):line (?<lineNumber>\d+))?\s*$",
            RegexOptions.Multiline | RegexOptions.ExplicitCapture);

        public static IEnumerable<StackFrame> Parse(Exception exception)
        {
            var stackFrames = new List<StackFrame>();
            ParseException(exception, stackFrames);
            return stackFrames;
        }

        private static void ParseException(Exception exception, List<StackFrame> stackFrames)
        {
            if (exception == null) return;
            stackFrames.AddRange(ParseStackTrace(exception.StackTrace));
            if (exception is AggregateException aggregateException)
            {
                var number = 0;
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    ParseInnerException(innerException, number, stackFrames);
                    number++;
                }
            }
            else if (exception.InnerException != null)
            {
                ParseInnerException(exception.InnerException, 0, stackFrames);
            }
        }

        private static void ParseInnerException(Exception innerException, int number, List<StackFrame> stackFrames)
        {
            var (namespaceName, className) = GetNamespaceNameAndClassName(innerException.GetType());
            var methodName = string.Empty;
            if (!string.IsNullOrEmpty(namespaceName))
            {
                methodName = $"{className}: {innerException.Message}";
                className = $"(Inner Exception #{number}) {namespaceName}";
            }
            else
            {
                className = $"(Inner Exception #{number}) {className}: {innerException.Message}";
            }
            stackFrames.Add(new StackFrame(className, methodName, "", 0));
            ParseException(innerException, stackFrames);
        }

        private static (string NamespaceName, string ClassName) GetNamespaceNameAndClassName(Type type)
        {
            var className = type.ToString();
            var namespaceName = type.Namespace;
            if (!string.IsNullOrEmpty(namespaceName))
            {
                className = className.Substring(namespaceName.Length + 1);
            }
            return (namespaceName, className);
        }

        private static IEnumerable<StackFrame> ParseStackTrace(string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace)) yield break;
            foreach (Match match in _regex.Matches(stackTrace))
            {
                var className = match.Groups["className"].Value;
                var methodName = match.Groups["methodName"].Value;
                var lineNumberGroup = match.Groups["lineNumber"];
                var lineNumber = lineNumberGroup.Success ? int.Parse(lineNumberGroup.Value) : 0;
                var fileNameGroup = match.Groups["fileName"];
                var fileName = fileNameGroup.Success && lineNumber > 0 ? fileNameGroup.Value : "<unknown>";
                yield return new StackFrame(className, methodName, fileName, lineNumber);
            }
        }
    }

    public class StackFrame
    {
        public string ClassName { get; }
        public string MethodName { get; }
        public string FileName { get; }
        public int LineNumber { get; }

        public string Symbol => string.IsNullOrEmpty(MethodName) ? ClassName : $"{ClassName}.{MethodName}";

        public StackFrame(string className, string methodName, string fileName, int lineNumber)
        {
            ClassName = className;
            MethodName = methodName;
            FileName = fileName;
            LineNumber = lineNumber;
        }
    }

    public static class CrashlyticsException
    {
        public static ExceptionModel CreateExceptionModel(this Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            var exceptionModel = new ExceptionModel(exception.GetType().ToString(), exception.Message)
            {
                StackTrace = StackTraceParser.Parse(exception)
                    .Select(frame => new global::Firebase.Crashlytics.StackFrame(frame.Symbol, frame.FileName, frame.LineNumber))
                    .ToArray()
            };
            return exceptionModel;
        }
    }
}
#endif