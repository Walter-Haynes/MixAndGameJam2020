using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utilities
{
	public static partial class MathUtils
	{
		/// <summary> Returns the absolute of *value*. </summary>
		/// <returns>The absolute of *value*.</returns>
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Abs(this float value) => Mathf.Abs(value);
		
		/// <summary> Returns the square root of *value*. </summary>
		/// <returns>The square root of *value*.</returns>
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sqrt(this float value) => Mathf.Sqrt(value);


		/// <summary> Returns the Unsigned angle between *from* and *to*. </summary>
		/// <returns> The Unsigned angle between *from* and *to*. </returns>
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleTo(this Vector2 from, in Vector2 to) => Vector2.Angle(from, to);
	}
}
