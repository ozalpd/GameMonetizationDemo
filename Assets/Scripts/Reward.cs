using Pops.Controllers;
using System.Collections;
using UnityEngine;

public class Reward : ObstCollisionDetect
{
    public int rewardPoint = 5;

    public override void OnPlayerCollision(PlayerController player)
    {
        GameManager.Score += rewardPoint;
    }
}
