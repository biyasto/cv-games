using UnityEngine;

namespace OneSDK
{
    public class UnityDebug : ILogger
    {
        public void Log(object message) =>
            Debug.Log(message);
        public void Log(object message, Object context) =>
            Debug.Log(message, context);
        public void LogWarning(object message) =>
            Debug.LogWarning(message);
        public void LogWarning(object message, Object context) =>
            Debug.LogWarning(message, context);
        public void LogError(object message) =>
            Debug.LogError(message);
        public void LogError(object message, Object context) =>
            Debug.LogError(message, context); 
        public void LogException(System.Exception message) =>
        Debug.LogException(message);
        public void LogException(System.Exception message, Object context) =>
            Debug.LogException(message, context);
        public void LogWarningFormat(string format, params object[] args) =>
            Debug.LogWarningFormat(format, args);
        public void LogWarningFormat(Object context, string format, params object[] args) =>
            Debug.LogWarningFormat(context, format, args);
        public void LogErrorFormat(Object context, string format, params object[] args) =>
            Debug.LogErrorFormat(context, format, args);
        public void LogErrorFormat( string format, params object[] args) =>
            Debug.LogErrorFormat(format, args);
        public void LogFormat( string format, params object[] args) => 
            Debug.LogFormat(format, args);
        public void LogFormat(Object context, string format, params object[] args) => 
            Debug.LogFormat(context, format, args);
        
    }
}