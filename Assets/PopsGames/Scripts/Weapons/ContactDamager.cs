using Pops.Controllers;
using UnityEngine;

namespace Pops.Weapons
{
    /// <summary>
    /// Contact damager weapon against player, decreases GameManager.Health.
    /// </summary>
    public class ContactDamager : AbstractWeapon
    {
        [Range(1, 100)]
        public int damageFactor = 5;
        [SerializeField]
        [Range(1f, 10f)]
        private float attackRange = 2f;

        public override float AttackRange
        {
            get { return attackRange; }
            set { attackRange = value; }
        }

        protected override void OnAttacking(Vector3 target, float prevAttackTime)
        {
            GameManager.Health -= damageFactor;
        }
    }
}