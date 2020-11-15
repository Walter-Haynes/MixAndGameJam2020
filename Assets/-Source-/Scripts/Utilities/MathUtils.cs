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
		
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float NormalToAngle(this Vector2 normal) => Mathf.Abs(Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg);
		
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DirectionToAngle(this Vector2 normal) => (Mathf.Atan2(normal.x, normal.y) * Mathf.Rad2Deg);
		
		[PublicAPI]
		public static Vector2 Parabola(in Vector2 start, in Vector2 stop, float height, in float percentage)
		{
			float __Func(float t) => -4 * height * t * t + 4 * height * t;

			Vector2 __midPoint = Vector2.Lerp(start, stop, percentage);

			return new Vector2(__midPoint.x, y: __Func(percentage) + Mathf.Lerp(start.y, stop.y, percentage));
		}
	}
}
