using UnityEngine;

using Sirenix.OdinInspector;

namespace Scripts.Game.Enemies
{
	public class BaseEnemy : MonoBehaviour
	{
		[InlineEditor]
		[SerializeField] protected EnemyAction[] enemyActions;

		protected void Start()
		{
			foreach (EnemyAction __action in enemyActions)
			{
				__action.Initialize();
			}
		}
	}
}    
