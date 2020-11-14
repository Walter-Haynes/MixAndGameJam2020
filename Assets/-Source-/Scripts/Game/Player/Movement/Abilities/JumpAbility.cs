using System.Runtime.CompilerServices;
using UnityEngine;
using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement.Abilities
{
	using Utilities;
	
	[DisallowMultipleComponent]
	public sealed class JumpAbility : PlayerAbility
	{
		#region Fields
		
		[Tooltip("Jump height of the character, regardless of Gravity")]
		[SerializeField] private float jumpHeight = 2;
		
		[UsedImplicitly]
		private bool _wannaJump = false;
		
		#endregion

		#region Methods

		private void OnEnable()
		{
			Player.Inputs.Jump.performed += _ => _wannaJump = true;
		}

		internal override void AbilityFixedUpdate()
		{
			base.AbilityFixedUpdate();
			
			if (_wannaJump)
			{
				TryJump();
				_wannaJump = false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryJump()
		{
			//TODO: (Walter) Beat check.

			if (!Player.IsGrounded) return false;

			// Calculate the velocity required to achieve the target jump height.
			float __jumpSpeed = Mathf.Sqrt(f: 2 * jumpHeight * Player.Gravity.y.Abs());
			if (Player.Gravity.y > 0)
			{
				__jumpSpeed *= -1;
			}
			
			Player.Move(y: __jumpSpeed);
			
			return true;
		}

		#endregion
	}
}
