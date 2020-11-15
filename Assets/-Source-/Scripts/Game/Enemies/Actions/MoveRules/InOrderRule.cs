using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Game.Enemies.Actions.MoveRules
{
    [CreateAssetMenu(menuName = "Enemies/Move Rule/Create InOrder Rule", fileName = "InOrderRule", order = 0)]
    public sealed class InOrderRule : MoveRule
    {
        [InlineEditor]
        [SerializeField] private MoveRule[] moveRules;
        
        private int _index = 0;
        public override void Do(in BaseEnemy enemy)
        {
            if(moveRules[_index] != null)
            {
                moveRules[_index].Do(enemy);
            }
            _index++;

            if(_index >= moveRules.Length)
            {
                _index = 0;
            }
        }
    }
}
