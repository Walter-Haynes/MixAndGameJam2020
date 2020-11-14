using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;
using Lean.Transition;
using UnityEngine.InputSystem;

namespace Scripts.Game.Player.Movement
{
	using Utilities;
	
	[DisallowMultipleComponent]
	public sealed class PlayerSideJump : PlayerAbility
	{
		#region Fields
		
		[SerializeField] private float sideJumpDuration = 0.5f;

		[SerializeField] private float sideJumpHeight = 2.0f;
		
		[UsedImplicitly]
		private bool _wannaJump = false;

		private bool HasJumpInput => (Player.Inputs.Jump.phase == InputActionPhase.Started);
		
		private bool HasMoveInput => (Player.Inputs.Left.phase  == InputActionPhase.Started ||
									  Player.Inputs.Right.phase == InputActionPhase.Started);
		private bool WannaSideJump => ((_wannaJump || HasJumpInput) && HasMoveInput);
		
		#endregion

		#region Methods

		private void OnEnable()
		{
			Player.Inputs.Jump.performed += _ => _wannaJump = true;
		}

		internal override void AbilityFixedUpdate()
		{
			base.AbilityFixedUpdate();
			
			if (WannaSideJump)
			{
				TrySideJump();
			}
			
			_wannaJump = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TrySideJump()
		{
			//TODO: (Walter) Beat check.
			if (!Player.IsGrounded) return false;

			Vector3 __startPos = transform.localPosition;

			bool __goLeft = (Player.Inputs.Movement.ReadValue<float>() < 0);
			
			if(__goLeft)
			{
				transform.localPositionTransition_X(position: __startPos.x - 3, duration: sideJumpDuration);
			}
			else
			{
				transform.localPositionTransition_X(position: __startPos.x + 3, duration: sideJumpDuration);
			}

			//hop
			transform
				.localPositionTransition_Y(position: __startPos.y + sideJumpHeight, duration: sideJumpDuration / 2.0f, ease: LeanEase.SineOut)
				.JoinTransition()
				.localPositionTransition_Y(position: __startPos.y                 , duration: sideJumpDuration / 2.0f, ease: LeanEase.SineOut);

			return true;
		}

		#endregion
	}
}