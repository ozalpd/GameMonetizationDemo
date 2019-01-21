using Pops.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pops.Weapons
{
    /// <summary>
    /// Time Damager weapon against player, decreases GameManager.Health.
    /// </summary>
    public class TimeDamager : AbstractWeapon
    {
        [Range(0.1f, 100f)]
        public float damageFactor = 2;

        [Tooltip("Invulnerable Period of Time")]
        public float invulnerablePeriod = 5f;

        private float attackRange = 0f;
        public override float AttackRange
        {
            get { return attackRange; }
            set { attackRange = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            GameManager.GameStateChanged += GameManager_GameStateChanged;
        }

        private void GameManager_GameStateChanged(GameState gameState)
        {
            if (gameState == GameState.Running)
            {
                InvokeRepeating("Attack", invulnerablePeriod, AttackInterval);
            }
            else
            {
                StopAttack();
            }
        }

        public void StartAttack()
        {
            InvokeRepeating("Attack", AttackTimeRemaining, AttackInterval);
        }

        public void StopAttack()
        {
            CancelInvoke("Attack");
        }

        private void Attack()
        {
            Attack(transform.forward);
        }

        protected override void OnAttacking(Vector3 target, float prevAttackTime)
        {
            GameManager.Health -= damageFactor;
        }
    }
}