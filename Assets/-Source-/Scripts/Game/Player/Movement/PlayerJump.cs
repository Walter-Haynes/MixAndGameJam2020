using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement
{
	using Utilities;
	
	[DisallowMultipleComponent]
	public sealed class PlayerJump : PlayerAbility
	{
		#region Fields
		
		[Tooltip("Jump height of the character, regardless of Gravity")]
		[SerializeField] private float jumpHeight = 4;
		
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
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryJump()
		{
			//TODO: (Walter) Beat check.

			if (!Player.IsGrounded)
			{
				_wannaJump = false;
				return false;
			}
			
			// Calculate the velocity required to achieve the target jump height.
			float __jumpSpeed = Mathf.Sqrt(f: 2 * jumpHeight * Physics2D.gravity.y.Abs());
			Player.Move(y: __jumpSpeed);

			_wannaJump = false;
			return true;
		}

		#endregion
	}
}
