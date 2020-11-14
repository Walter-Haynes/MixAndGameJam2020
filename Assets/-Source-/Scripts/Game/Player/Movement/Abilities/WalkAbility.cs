using System.Runtime.CompilerServices;
using UnityEngine;
using JetBrains.Annotations;

using Lean.Transition;

namespace Scripts.Game.Player.Movement.Abilities
{

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
			
			Vector3 __playerStartPos = Player.transform.localPosition;
			
			if (_wannaWalkLeft)
			{
				transform.localPositionTransition_X(position: __playerStartPos.x - 1, duration: hopDuration);
			}
			else
			{
				transform.localPositionTransition_X(position: __playerStartPos.x + 1, duration: hopDuration);
			}

			Vector3 __visualsStartPos = Player.visuals.localPosition;
			float __hopY = 0.25f * (Player.HasNormalGravity ? 1 : -1);
			//hop
			Player.visuals
				.localPositionTransition_Y(position: __visualsStartPos.y + __hopY, duration: hopDuration / 2.0f, ease: LeanEase.SineOut)
				.JoinTransition()
				.localPositionTransition_Y(position: __visualsStartPos.y,          duration: hopDuration / 2.0f, ease: LeanEase.SineOut);

			return true;
		}

		#endregion
	}
}