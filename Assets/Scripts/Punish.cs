using Pops.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punish : ObstCollisionDetect
{
    public int damageFactor = 5;

    public override void OnPlayerCollision(PlayerController player)
    {
        GameManager.Damage += damageFactor;
        player.OnObstacleHit();
    }
}
