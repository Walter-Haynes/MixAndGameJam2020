using Lean.Transition;
using UnityEngine;

namespace Scripts.Game.Enemies
{
    [CreateAssetMenu(menuName = "Enemies/Actions/Create ScaleAction", fileName = "ScaleAction", order = 0)]
    public sealed class ScaleAction : EnemyAction
    {
        [SerializeField] private Vector3 scaleTo = Vector3.one * 1.5f;

        [SerializeField] private float scaleDuration = 0.1f;
        
        protected override void React()
        {
            Enemy.transform
                .localScaleTransition(scale: scaleTo, duration: scaleDuration / 2.0f, ease: LeanEase.Accelerate)
                .JoinTransition()
                .localScaleTransition(scale: Vector3.one, duration: scaleDuration / 2.0f, LeanEase.BounceOut);
        }
    }
}
