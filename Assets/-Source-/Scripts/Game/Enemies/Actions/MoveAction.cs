using Lean.Transition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Game.Enemies.Actions
{
	
	
	[CreateAssetMenu(menuName = "Enemies/Actions/Create Move Action", fileName = "MoveAction", order = 0)]
	public sealed class MoveAction : EnemyAction
	{
		[BoxGroup("Move")]
		[InlineEditor]
		[SerializeField] private MoveRule moveRule;
        
		protected override void React()
		{
			moveRule.Do(Enemy);
		}
	}
}