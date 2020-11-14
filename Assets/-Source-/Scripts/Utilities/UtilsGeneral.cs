using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utilities
{
	public static partial class UtilsGeneral
	{
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Log(this string message) => Debug.Log(message);
		
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void LogWarning(this string message) => Debug.LogWarning(message);
		
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void LogError(this string message) => Debug.LogError(message);
		
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assert(this bool condition, in string message) => Debug.Assert(condition: condition, message:message);
	}
}