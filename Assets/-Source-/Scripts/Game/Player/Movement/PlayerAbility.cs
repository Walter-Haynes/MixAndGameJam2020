namespace Scripts.Game.Player.Movement
{
	[ExecuteAfter(typeof(PlayerController2D))]
	/// <summary> Inherit Abilities from this, so the PlayerController2D can call the Ability's Update and FixedUpdate methods. </summary>
	public abstract class PlayerAbility : PlayerComponent
	{
		internal virtual void AbilityUpdate()
		{
			
		}

		internal virtual void AbilityFixedUpdate()
		{
			
		}
	}
}
