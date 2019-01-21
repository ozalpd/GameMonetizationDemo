using Pops.Weapons;
using UnityEngine;

//For Attacking State
namespace Pops.Controllers.NPC
{
    public abstract partial class NpcController
    {
        //[Header("Attack State Settings")]


        public float AttackDistance { get { return Weapon != null ? Weapon.AttackDistance : 0f; } }

        protected override void OnAttacking()
        {
            NavObstacleMode = true;
            base.OnAttacking();
        }

        protected override void OnAttackStopped()
        {
            NavAgentMode = true;
            base.OnAttackStopped();
        }

        /// <summary>
        /// Executed when state changed to Attack
        /// </summary>
        protected virtual void OnStateChangedToAttack()
        {
            Attacking = true;
        }

        protected virtual void UpdateAttacking()
        {
            Attacking = WatchTargetDistance < AttackRange;
            if (Attacking)
            {
                if (NavObstacleMode)
                    LookToward(WatchTarget.position);
            }
            else
            {
                State = NpcState.Chasing;
            }
        }
    }
}
