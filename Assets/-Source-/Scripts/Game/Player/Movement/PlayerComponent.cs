using UnityEngine;
using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement
{
	
	/// <summary> Inherit Player Abilities / Components from this. </summary>
	[RequireComponent(typeof(PlayerController2D))]
	public abstract class PlayerComponent : MonoBehaviour
	{
		[UsedImplicitly]
		protected PlayerController2D Player { get; private set; }

		protected virtual void Awake()
		{
			if (Player != null) return;
        
			if(TryGetComponent(component: out PlayerController2D __result))
			{
				Player = __result;
			}
		}

		protected virtual void Reset()
		{
			if(TryGetComponent(component: out PlayerController2D __result))
			{
				Player = __result;
			}
		}
		
		/*
		protected virtual void OnEnable()
		{
			
		}

		protected virtual void OnDisable()
		{
			
		}
		*/

	}
}
