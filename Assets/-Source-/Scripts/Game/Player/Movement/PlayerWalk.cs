using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;
using Lean.Transition;
using UnityEngine.InputSystem;

namespace Scripts.Game.Player.Movement
{
	using Utilities;
	
	[DisallowMultipleComponent]
	[RequireComponent(typeof(PlayerController2D))]
	public sealed class PlayerWalk : PlayerAbility
	{
		#region Fields

		[SerializeField] private float hopDuration = 0.05f;

		[SerializeField] private float jumpHopDuration = 0.5f;
		
		[UsedImplicitly]
		private bool _wannaWalkLeft = false, _wannaWalkRight = false;
		[UsedImplicitly]
		private bool _wannaJump = false;
		
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
			if(_wannaWalkRight)
			{
				TryWalk(left: false);
			}
			else if(_wannaWalkLeft)
			{
				TryWalk(left: true);
			}

			//reset
			_wannaWalkLeft = false;
			_wannaWalkRight = false;
			_wannaJump = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryWalk(bool left)
		{
			//TODO: (Walter) Beat check.

			if(!Player.IsGrounded) return false;
			
			Vector3 __startPos = transform.localPosition;
			
			if (Player.Inputs.Jump.phase == InputActionPhase.Started)
			{
				__JumpHop();	
			}
			else
			{
				__WalkHop();	
			}
			
			return true;

			void __WalkHop()
			{
				if (left)
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
			}
			
			void __JumpHop()
			{
				if (left)
				{

					transform.localPositionTransition_X(position: __startPos.x - 3, duration: jumpHopDuration);
				}
				else
				{
					transform.localPositionTransition_X(position: __startPos.x + 3, duration: jumpHopDuration);
				}
			
				//hop
				transform
					.localPositionTransition_Y(position: __startPos.y + 2, duration: jumpHopDuration / 2.0f, ease: LeanEase.SineOut)
					.JoinTransition()
					.localPositionTransition_Y(position: __startPos.y,     duration: jumpHopDuration / 2.0f, ease: LeanEase.SineOut);
			}
		}

		private bool TryJumpHop()
		{
			
		}

		#endregion
	}
}