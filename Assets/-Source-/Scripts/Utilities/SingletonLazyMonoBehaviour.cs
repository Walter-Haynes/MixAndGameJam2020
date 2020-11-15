using System;
using UnityEngine;
using JetBrains.Annotations;

namespace Scripts.Utilities
{
	/// <summary>
	/// Lazy Singleton based on this: https://blog.mzikmund.com/2019/01/a-modern-singleton-in-unity/
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonLazyMonoBehaviour<T> : MonoBehaviour 
		where T : SingletonLazyMonoBehaviour<T>
	{
		private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateSingleton);

		[PublicAPI]
		public static T Instance => LazyInstance.Value;

		private static T CreateSingleton()
		{
			GameObject __ownerObject = new GameObject(name: $"[{typeof(T).Name}]");
			T __instance = __ownerObject.AddComponent<T>();
			DontDestroyOnLoad(__ownerObject);
			return __instance;
		}
	}
}