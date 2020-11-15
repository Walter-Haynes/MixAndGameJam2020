using System;
using UnityEngine;

namespace Scripts.Game.Enemies.Actions
{

    public abstract class MoveRule : ScriptableObject
    {
        public abstract void Do(in BaseEnemy enemy);
    }
}
