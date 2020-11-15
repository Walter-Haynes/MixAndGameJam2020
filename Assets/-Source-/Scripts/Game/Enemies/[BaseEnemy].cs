using UnityEngine;

using Sirenix.OdinInspector;

namespace Scripts.Game.Enemies
{
	public abstract class BaseEnemy : MonoBehaviour
	{
		[BoxGroup]
		[InlineEditor]
		[SerializeField] protected EnemyAction[] enemyActions;

		[BoxGroup]
		[SceneObjectsOnly] 
		[SerializeField] internal Transform visuals; 

		protected void Start()
		{
			foreach (EnemyAction __action in enemyActions)
			{
				if(__action != null)
				{
					__action.Initialize(enemy: this);	
				}
			}
		}
	}
}    
