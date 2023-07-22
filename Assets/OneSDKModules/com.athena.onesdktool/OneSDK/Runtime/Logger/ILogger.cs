
using UnityEngine;
namespace OneSDK
{
	public interface ILogger
	{
		void Log(object message);
		void Log(object message, Object context);
		void LogWarning(object message);
		void LogWarning(object message, Object context);
		void LogError(object message);
		void LogError(object message, Object context);
		void LogException (System.Exception message);
		void LogException (System.Exception message, Object context);
		void LogWarningFormat(string format, params object[] args);
		void LogWarningFormat(Object context, string format, params object[] args);
		void LogErrorFormat(string format, params object[] args);
		void LogErrorFormat(Object context, string format, params object[] args);
		void LogFormat(string format, params object[] args);
		void LogFormat(Object context, string format, params object[] args);
	}
}