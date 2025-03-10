using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadLevelManager
{
	public class MadDebug
	{
		public class AssertException : Exception
		{
			public AssertException(string message)
				: base(message)
			{
			}
		}

		public const string internalPostfix = "\nThis is an internal error. Please report this to support@madpixelmachine.com";

		private static HashSet<string> messages = new HashSet<string>();

		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new AssertException(message);
			}
		}

		public static void Log(string message)
		{
			Log(message, null);
		}

		public static void Log(string message, UnityEngine.Object context)
		{
		}

		public static void LogOnce(string message)
		{
			LogOnce(message, null);
		}

		public static void LogOnce(string message, UnityEngine.Object context)
		{
			if (!messages.Contains(message))
			{
				messages.Add(message);
				UnityEngine.Debug.Log(message, context);
			}
		}

		public static void LogWarningOnce(string message)
		{
			LogWarningOnce(message, null);
		}

		public static void LogWarningOnce(string message, UnityEngine.Object context)
		{
			if (!messages.Contains(message))
			{
				messages.Add(message);
				UnityEngine.Debug.LogWarning(message, context);
			}
		}

		public static void LogErrorOnce(string message)
		{
			LogErrorOnce(message, null);
		}

		public static void LogErrorOnce(string message, UnityEngine.Object context)
		{
			if (!messages.Contains(message))
			{
				messages.Add(message);
				UnityEngine.Debug.LogError(message, context);
			}
		}

		public static void Internal(string message)
		{
			UnityEngine.Debug.LogError(message + "\nThis is an internal error. Please report this to support@madpixelmachine.com");
		}

		public static void Internal(string message, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogError(message + "\nThis is an internal error. Please report this to support@madpixelmachine.com", context);
		}
	}
}
