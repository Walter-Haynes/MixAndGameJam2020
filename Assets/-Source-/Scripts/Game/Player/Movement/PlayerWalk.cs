using System;
using System.Runtime.CompilerServices;

using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement
{
	using Utilities;
	
	[DisallowMultipleComponent]
	[RequireComponent(typeof(PlayerController2D))]
	public sealed class PlayerWalk : PlayerComponent
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

		private void FixedUpdate()
		{
			
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryWalk()
		{
			
			return false;
		}

		#endregion
	}
}