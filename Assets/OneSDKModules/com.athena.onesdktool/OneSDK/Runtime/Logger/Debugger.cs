

using System;
using UnityEngine;

namespace OneSDK
{
    public static class Debugger
    {
        private static ILogger loggingSolution => new UnityDebug();

        private static ILogger s_logger;
        private static ILogger logger => s_logger ?? loggingSolution;

        private const string ERROR_COLOR_CODE = "#D9534F";
        private const string INFO_COLOR_CODE = "#1C7CD5";
        private const string OK_COLOR_CODE = "#5CB85C";
        private const string WARNING_COLOR_CODE = "#EE9800";

        public enum LogType
        {
            Assert,
            Error,
            Warning,
            Log,
            Exception,
        }

        private static string NhimDevPrefix(LogType logType)
        {
            string colorCode = "#121212";
            switch (logType)
            {
                case LogType.Log:
                    colorCode = INFO_COLOR_CODE;
                    break;
                case LogType.Warning:
                    colorCode = WARNING_COLOR_CODE;
                    break;
                case LogType.Error:
                    colorCode = ERROR_COLOR_CODE;
                    break;
                case LogType.Exception:
                    colorCode = ERROR_COLOR_CODE;
                    break;
                case LogType.Assert:
                    colorCode = OK_COLOR_CODE;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }

            return $"<color={colorCode}><b>ONE_SDK ››› </b></color>";
        }

        public static void Log(object message, UnityEngine.Object context = null)
        {
            message = NhimDevPrefix(LogType.Log) + message;
            logger.Log(message, context);
        }

        public static void LogWarning(object message, UnityEngine.Object context = null)
        {
            message = NhimDevPrefix(LogType.Warning) + message;
            logger.LogWarning(message, context);
        }

        public static void LogError(object message, UnityEngine.Object context = null)
        {
            message = NhimDevPrefix(LogType.Error) + message;
            logger.LogError(message, context);
        }
        
        public static void LogException(System.Exception message, UnityEngine.Object context = null)
        {
            var new_message = $"{NhimDevPrefix(LogType.Exception)}{message.StackTrace}";
            logger.LogException(new Exception(new_message), context);
        }
        public static void LogFormat(string format, UnityEngine.Object context = null, params object[] args)
        {
            string new_message = NhimDevPrefix(LogType.Log) + format;
            logger.LogFormat(context, new_message, args);
        }
        public static void LogWarningFormat(string format, UnityEngine.Object context = null, params object[] args)
        {
            string new_message = NhimDevPrefix(LogType.Warning) + format;
            logger.LogWarningFormat(context, new_message, args);
        }
        public static void LogErrorFormat(string format, UnityEngine.Object context = null, params object[] args)
        {
            string new_message = NhimDevPrefix(LogType.Error) + format;
            logger.LogErrorFormat(context, new_message, args);
        }

    }
}