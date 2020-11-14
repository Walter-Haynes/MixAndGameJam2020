using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;
using Lean.Transition;
using UnityEngine.InputSystem;

namespace Scripts.Game.Player.Movement
{
	using Utilities;
	
	[DisallowMultipleComponent]
	public sealed class WalkAbility : PlayerAbility
	{
		#region Fields

		[SerializeField] private float hopDuration = 0.05f;

		[UsedImplicitly]
		private bool _wannaWalkLeft = false, _wannaWalkRight = false;
		[UsedImplicitly]
		private bool _wannaJump = false;

		private bool WannaWalk => (_wannaWalkLeft || _wannaWalkRight);
		
		#endregion

		#region Methods

		private void OnEnable()
		{
			Player.Inputs.Left.performed  += _ => _wannaWalkLeft = true;
			Player.Inputs.Right.performed += _ => _wannaWalkRight = true;
			
			Player.Inputs.Jump.performed += _ => _wannaJump = true;
		}

		internal override void AbilityFixedUpdate()
		{
			if (!_wannaJump)
			{
				if(WannaWalk)
				{
					TryWalk();
				}
			}

			//reset
			_wannaWalkLeft = false;
			_wannaWalkRight = false;
			_wannaJump = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryWalk()
		{
			//TODO: (Walter) Beat check.

			if(!Player.IsGrounded) return false;
			
			Vector3 __startPos = transform.localPosition;
			
			if (_wannaWalkLeft)
			{

				transform.localPositionTransition_X(position: __startPos.x - 1, duration: hopDuration);
			}
			else
			{
				transform.localPositionTransition_X(position: __startPos.x + 1, duration: hopDuration);
			}
			
			//hop
			transform
				.localPositionTransition_Y(position: __startPos.y + 0.25f, duration: hopDuration / 2.0f, ease: LeanEase.SineOut)
				.JoinTransition()
				.localPositionTransition_Y(position: __startPos.y,         duration: hopDuration / 2.0f, ease: LeanEase.SineOut);

			return true;
		}

		#endregion
	}
}