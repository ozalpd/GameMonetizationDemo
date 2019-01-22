using Pops.Controllers;
using System.Collections;
using UnityEngine;

public class Reward : ObstCollisionDetect
{
    public int rewardPoint = 5;


    protected override void CheckCollision(GameObject gameObject)
    {
        base.CheckCollision(gameObject);
        if (gameObject.tag.Equals("Player"))
            GameManager.Score += rewardPoint;
    }
}
