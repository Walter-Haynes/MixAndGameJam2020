using UnityEngine;

using Sirenix.OdinInspector;

using Lean.Transition;

namespace Scripts.Game.Enemies
{
    [CreateAssetMenu(menuName = "Enemies/Actions/Create ScaleAction", fileName = "ScaleAction", order = 0)]
    public sealed class ScaleAction : EnemyAction
    {
        [BoxGroup("Scale")]
        [SerializeField] private Vector3 scaleTo = Vector3.one * 1.5f;

        [BoxGroup("Scale")]
        [SerializeField] private float scaleDuration = 0.1f;
        
        protected override void React()
        {
            Enemy.visuals
                .localScaleTransition(scale: scaleTo,     duration: scaleDuration / 2.0f, ease: LeanEase.Accelerate)
                .JoinTransition()
                .localScaleTransition(scale: Vector3.one, duration: scaleDuration / 2.0f, ease: LeanEase.BounceOut);
        }
    }
}
