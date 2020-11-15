using System.Collections;
using Scripts.Utilities;
using UnityEngine;

using Sirenix.OdinInspector;

namespace Scripts.Game.Enemies
{
	[CreateAssetMenu(menuName = "Enemies/Actions/Create ShootBullet Action", fileName = "ShootBulletAction", order = 0)]
	public sealed class ShootBulletAction : EnemyAction
	{
		[BoxGroup("Shoot")]
		[ValueDropdown(valuesGetter: "_directions")]
		[SerializeField] private Vector2 direction = Vector2.left;
		
		[BoxGroup("Shoot")]
		[AssetsOnly]
		[SerializeField] private GameObject bulletPrefab;

		private static IEnumerable _directions = new ValueDropdownList<Vector2>
		{
			{"Up",    Vector2.up}, 
			{"Down",  Vector2.down},
			{"Left",  Vector2.left}, 
			{"Right", Vector2.right} 
		};
        
		protected override void React()
		{
			Instantiate(bulletPrefab, position: Enemy.transform.position + (Vector3)direction, rotation: Quaternion.AngleAxis(angle: direction.DirectionToAngle(), -Enemy.transform.forward));
		}
	}
}