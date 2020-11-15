using System.Runtime.CompilerServices;
using Lean.Transition;
using UnityEngine;

namespace Scripts.Game.Enemies.Actions.MoveRules
{
	[CreateAssetMenu(menuName = "Enemies/Move Rule/Create WalkUntilBounce Rule", fileName = "WalkUntilBounceRule", order = 0)]
	public abstract class WalkUntil : MoveRule
	{

		[SerializeField] private Vector2 walkDirection = Vector2.right;
		
		[SerializeField] private float hopDuration = 0.15f;
		[SerializeField] private float hopHeight = 0.3f;
		
		public override void Do(in BaseEnemy enemy)
		{
			Debug.Log("Walk");
			Walk(enemy);
		}

		protected virtual void Walk(in BaseEnemy enemy)
		{
			Vector2 __targetPos = GetNextTilePos(enemy);
			
			enemy.transform.positionTransition(position: __targetPos, duration: hopDuration, ease: LeanEase.Smooth);

			Vector3 __visualsStartPos = enemy.visuals.localPosition;
			enemy.visuals
				.localPositionTransition_Y(position: __visualsStartPos.y + hopHeight, duration: hopDuration / 2.0f, ease: LeanEase.SineOut)
				.JoinTransition()
				.localPositionTransition_Y(position: __visualsStartPos.y,             duration: hopDuration / 2.0f, ease: LeanEase.SineOut);
		}

		protected Vector2 direction = Vector2.right;

		private Vector2 GetNextTilePos(in BaseEnemy enemy)
		{
			Vector2 __targetPos = enemy.transform.position;
			__targetPos += direction;
			
			Collider2D __hitWall = Physics2D.OverlapCircle(point: __targetPos, radius: 0.15f);

			if(__hitWall)
			{
				__targetPos = TargetOnHitWall(enemy: enemy);
			}

			return __targetPos;
		}

		/// <summary> What behaviour to perform when there is a wall at our target pos. </summary>
		/// <returns>A new target pos</returns>
		protected abstract Vector2 TargetOnHitWall(in BaseEnemy enemy);
	}
}