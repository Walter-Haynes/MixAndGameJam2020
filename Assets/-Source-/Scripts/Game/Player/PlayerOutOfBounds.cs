using System.Collections;
using System.Collections.Generic;
using Scripts.Game.Player.Movement;
using UnityEngine;

public class PlayerOutOfBounds : MonoBehaviour
{
    private void Update()
    {
        if (!PlayerController2D.InstanceExists) return;
        
        PlayerController2D __player = PlayerController2D.Instance;

        if (__player.transform.position.y > 25 || __player.transform.position.y < -25)
        {
            __player.Health.Kill();   
        }
    }
}
