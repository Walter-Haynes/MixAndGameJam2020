using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Utilities
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour 
		where T : SingletonMonoBehaviour<T>
	{
		#region Fields & Properties

		private static T _instance = null;
		
		[PublicAPI]
		public static T Instance
		{
			get => _instance = InstanceExists ? _instance : FindObjectOfType<T>();
			protected set => _instance = value;
		}
		
		[PublicAPI]
		public static bool InstanceExists => (_instance != null);

		#endregion

		#region Methods
		
		protected virtual void OnEnable()
		{
			if(InstanceExists)
			{
				Destroy(Instance.gameObject);
			}
			
			Instance = (T)this;
		}
		
		protected virtual void OnDisable()
		{
			if(Instance == this)
			{
				Instance = null;
			}
		}

		#endregion
	}
}