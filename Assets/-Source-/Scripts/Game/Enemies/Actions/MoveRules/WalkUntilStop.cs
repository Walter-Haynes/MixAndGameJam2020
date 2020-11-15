using UnityEngine;

namespace Scripts.Game.Enemies.Actions.MoveRules
{
	[CreateAssetMenu(menuName = "Enemies/Move Rule/Create WalkUntilStop Rule", fileName = "WalkUntilStopRule", order = 0)]
	public sealed class WalkUntilStop : WalkUntil
	{
		protected override Vector2 TargetOnHitWall(in BaseEnemy enemy)
		{
			return enemy.transform.position;
		}
	}
}