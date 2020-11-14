using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement.Abilities
{

	[DisallowMultipleComponent]
	public sealed class GravityFlipAbility : PlayerAbility
	{
		#region Fields

		[UsedImplicitly]
		private bool _wannaFlip = false;
		
		#endregion

		#region Methods

		private void OnEnable()
		{
			Player.Inputs.Flip.performed += _ => _wannaFlip = true;
		}

		internal override void AbilityFixedUpdate()
		{
			base.AbilityFixedUpdate();
			
			if (_wannaFlip)
			{
				TryFlip();
			}

			_wannaFlip = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryFlip()
		{
			Debug.Log("FLIP!");
			
			//TODO: (Walter) Beat check.
			Player.Gravity *= -1;

			Vector3 __currentRotation = Player.transform.rotation.eulerAngles;
			Player.transform.rotation = Quaternion.Euler(__currentRotation.x, __currentRotation.y, __currentRotation.z + 180);
			//Player.IsGrounded = false;

			return true;
		}

		#endregion
	}
}