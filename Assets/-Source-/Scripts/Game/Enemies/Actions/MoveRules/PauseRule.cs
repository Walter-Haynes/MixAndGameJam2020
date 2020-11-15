using UnityEngine;

namespace Scripts.Game.Enemies.Actions.MoveRules
{
	[CreateAssetMenu(menuName = "Enemies/Move Rule/Create Pause Rule", fileName = "PauseRule", order = 0)]
	public sealed class PauseRule : MoveRule
	{
		public override void Do(in BaseEnemy enemy)
		{
            //Literally nothing.
		}
	}
}