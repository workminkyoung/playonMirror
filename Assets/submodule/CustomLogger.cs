using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomLogger
{
    public static bool LogEnabled { get; set; } = true;
    public static LogType FilterLogType { get; set; } = LogType.Log;


    private static readonly Dictionary<LogType, string> LogTypeToText = new Dictionary<LogType, string>
    {
            { LogType.Log, "INFO" },
            { LogType.Warning, "WARNING" },
            { LogType.Error, "ERROR" },
            { LogType.Exception, "EXCEPTION" },
            { LogType.Assert, "ASSERT" }
    };


    private static string GetLogLevelPrefix(LogType logType)
    {
        return LogTypeToText[LogType.Log];
    }


    private static string GetTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    public static bool IsLogTypeAllowed(LogType logType)
    {
        return LogEnabled && (logType <= FilterLogType);
    }

    public static void Log(LogType logType, object message)
    {
        if (IsLogTypeAllowed(logType))
        {
            Debug.unityLogger.LogFormat(logType, "[{0}] [{1}] {2}", GetLogLevelPrefix(logType), GetTimestamp(), message);
        }
    }

    public static void Log(LogType logType, object message, UnityEngine.Object context)
    {
        if (IsLogTypeAllowed(logType))
        {
            Debug.unityLogger.LogFormat(logType, context, "[{0}] [{1}] {2}", GetLogLevelPrefix(logType), GetTimestamp(), message);
        }
    }

    public static void Log(LogType logType, string tag, object message)
    {
        if (IsLogTypeAllowed(logType))
        {
            Debug.unityLogger.LogFormat(logType, "[{0}] [{1}]{2} - {3}", GetLogLevelPrefix(logType), GetTimestamp(), tag, message);
        }
    }

    public static void Log(LogType logType, string tag, object message, UnityEngine.Object context)
    {
        if (IsLogTypeAllowed(logType))
        {
            Debug.unityLogger.LogFormat(logType, context, "[{0}] [{1}]{2} - {3}", GetLogLevelPrefix(logType), GetTimestamp(), tag, message);
        }
    }

    public static void Log(object message)
    {
        Log(LogType.Log, message);
    }

    public static void Log(string tag, object message)
    {
        Log(LogType.Log, tag, message);
    }

    public static void LogWarning(object message)
    {
        Log(LogType.Warning, message);
    }

    public static void LogWarning(string tag, object message)
    {
        Log(LogType.Warning, tag, message);
    }

    public static void LogWarning(string tag, object message, UnityEngine.Object context)
    {
        Log(LogType.Warning, tag, message, context);
    }

    public static void LogError(object message)
    {
        Log(LogType.Error, message);
    }

    public static void LogError(string tag, object message)
    {
        Log(LogType.Error, tag, message);
    }

    public static void LogError(string tag, object message, UnityEngine.Object context)
    {
        Log(LogType.Error, tag, message, context);
    }

    public static void LogException(Exception exception)
    {
        if (LogEnabled && LogType.Exception <= FilterLogType)
        {
            CustomLogger.LogException(exception);
        }
    }

    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        if (LogEnabled && LogType.Exception <= FilterLogType)
        {
            CustomLogger.LogException(exception, context);
        }
    }

    public static void LogFormat(LogType logType, string format, params object[] args)
    {
        if (IsLogTypeAllowed(logType))
        {
            Debug.unityLogger.LogFormat(logType, "{0} - " + format, GetTimestamp(), args);
        }
    }

    public static void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (IsLogTypeAllowed(logType))
        {
            Debug.unityLogger.LogFormat(logType, context, "{0} - " + format, GetTimestamp(), args);
        }
    }
}
