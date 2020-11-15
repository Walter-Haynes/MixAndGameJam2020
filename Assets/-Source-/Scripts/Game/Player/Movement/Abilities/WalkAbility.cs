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

		[SerializeField] private float hopDuration = 0.15f;
		[SerializeField] private float hopHeight = 0.3f;

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
			
			Vector3 __playerStartPos = Player.transform.position;

			(bool __canHop, Vector3 __targetPos) = GetNextPos();

			if (!__canHop) return false;

			transform.positionTransition(position: __targetPos, duration: hopDuration, ease: LeanEase.Smooth);
			
			
			/*
			if (_wannaWalkLeft)
			{
				transform.localPositionTransition_X(position: __playerStartPos.x - 1, duration: hopDuration);
			}
			else
			{
				transform.localPositionTransition_X(position: __playerStartPos.x + 1, duration: hopDuration);
			}
			*/

			Vector3 __visualsStartPos = Player.visuals.localPosition;
			//float __hopY = hopHeight * (Player.HasNormalGravity ? 1 : -1);
			
			//hop
			Player.visuals
				.localPositionTransition_Y(position: __visualsStartPos.y + hopHeight, duration: hopDuration / 2.0f, ease: LeanEase.SineOut)
				.JoinTransition()
				.localPositionTransition_Y(position: __visualsStartPos.y,             duration: hopDuration / 2.0f, ease: LeanEase.SineOut);

			return true;
		}

		private (bool canHop, Vector3 targetPos) GetNextPos()
		{
			Vector3 __checkPos = Player.transform.position;
			__checkPos.x += _wannaWalkLeft ? -1 : 1;
			
			Collider2D __isOccupied = Physics2D.OverlapCircle(point: __checkPos, radius: 0.15f);

			if(__isOccupied)
			{
				__checkPos.y += Player.HasNormalGravity ? 1 : -1;
				
				__isOccupied = Physics2D.OverlapCircle(point: __checkPos, radius: 0.15f);
			}

			return (canHop: !__isOccupied, __checkPos);
		}

		#endregion
	}
}