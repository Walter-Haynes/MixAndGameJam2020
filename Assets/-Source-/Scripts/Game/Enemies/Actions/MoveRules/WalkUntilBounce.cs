using UnityEngine;

namespace Scripts.Game.Enemies.Actions.MoveRules
{
    [CreateAssetMenu(menuName = "Enemies/Move Rule/Create WalkUntilBounce Rule", fileName = "WalkUntilBounceRule", order = 0)]
    public sealed class WalkUntilBounce : WalkUntil
    {
        protected override Vector2 TargetOnHitWall(in BaseEnemy enemy)
        {
            direction = -direction; //flip direction
            Vector2 __targetPos = enemy.transform.position;
            __targetPos += direction;

            return __targetPos;
        }
    }
}
