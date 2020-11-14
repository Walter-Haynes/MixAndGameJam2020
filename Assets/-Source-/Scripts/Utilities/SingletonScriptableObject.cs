using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Utilities
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject 
		where T : SingletonScriptableObject<T>
	{
		private static T _instance;

		[PublicAPI]
		public static T Instance
		{
			get
			{
				if (_instance != null) return _instance;

				T[] __instances = Resources.FindObjectsOfTypeAll<T>();

				if (__instances.Length == 0)
				{
					Debug.LogError(message: $"No Instance of type {typeof(T)} exists!");
					return null;
				}

				if (__instances.Length > 1)
				{
					Debug.LogError(message: $"More than one Instance of type {typeof(T)} exists! \n" +
											 "Returning the first one.");
				}

				_instance = __instances[0];
				_instance.hideFlags = HideFlags.DontUnloadUnusedAsset;

				return _instance;
			}
		}
		
	}
}