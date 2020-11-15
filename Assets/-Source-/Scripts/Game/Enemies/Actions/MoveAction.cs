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
		[SerializeField] private MoveRule[] moveRules;

		public override void Initialize(in BaseEnemy enemy)
		{
			base.Initialize(in enemy);

			_index = 0;
		}

		private int _index = 0;
		protected override void React()
		{
			if(moveRules == null) return;
			
			if(moveRules[_index] != null)
			{
				moveRules[_index].Do(Enemy);
			}
			_index++;

			if(_index >= moveRules.Length)
			{
				_index = 0;
			}
		}
	}
}