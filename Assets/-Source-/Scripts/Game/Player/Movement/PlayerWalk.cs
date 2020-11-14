using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;
using Lean.Transition;

namespace Scripts.Game.Player.Movement
{
	using Utilities;
	
	[DisallowMultipleComponent]
	[RequireComponent(typeof(PlayerController2D))]
	public sealed class PlayerWalk : PlayerAbility
	{
		#region Fields

		[UsedImplicitly]
		private bool _wannaWalkLeft = false, _wannaWalkRight = false;
		
		#endregion

		#region Methods

		private void OnEnable()
		{
			Player.Inputs.Left.performed  += _ => _wannaWalkLeft = true;
			Player.Inputs.Right.performed += _ => _wannaWalkRight = true;
		}

		internal override void AbilityFixedUpdate()
		{
			if(_wannaWalkRight)
			{
				TryWalk(left: false);
			}
			else if(_wannaWalkLeft)
			{
				TryWalk(left: true);
			}

			//reset
			_wannaWalkLeft = _wannaWalkRight = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryWalk(in bool left)
		{
			//TODO: (Walter) Beat check.

			if(!Player.IsGrounded) return false;

			if (left)
			{
				transform.localPositionTransition_X(position: transform.localPosition.x - 1, duration: 0.05f);
			}
			else
			{
				transform.localPositionTransition_X(position: transform.localPosition.x + 1, duration: 0.05f);
			}
			
			return true;
		}

		#endregion
	}
}