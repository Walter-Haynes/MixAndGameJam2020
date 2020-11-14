using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utilities
{
	public static partial class MathUtils
	{
		[PublicAPI]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Abs(this float value) => Mathf.Abs(value);
	}
}
