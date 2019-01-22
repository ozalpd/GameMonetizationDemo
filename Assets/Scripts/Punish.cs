using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punish : ObstCollisionDetect
{
    public override void OnPlayerCollision(PlayerController player)
    {
        player.OnObstacleHit();
    }
}
