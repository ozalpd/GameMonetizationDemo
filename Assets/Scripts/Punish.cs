using Pops.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punish : ObstCollisionDetect
{
    public int damageFactor = 5;

    private new Collider collider;

    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<Collider>();

        GameManager.InvulnerabilityChanged += GameManager_InvulnerabilityChanged;
    }

    private void OnDestroy()
    {
        GameManager.InvulnerabilityChanged -= GameManager_InvulnerabilityChanged;
    }

    private void GameManager_InvulnerabilityChanged(bool invulnerable)
    {
        collider.isTrigger = invulnerable;
    }


    protected override void CheckCollision(GameObject gameObject)
    {
        if (GameManager.Invulnerable)
            return;

        base.CheckCollision(gameObject);
    }

    public override void OnPlayerCollision(PlayerController player)
    {
        GameManager.Damage += damageFactor;
        player.OnObstacleHit();
    }
}
