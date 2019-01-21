using Pops.Extensions;
using UnityEngine;

//For Chasing State
namespace Pops.Controllers.NPC
{
    public abstract partial class NpcController
    {
        [Header("Chase State Settings")]
        public float chaseSpeed = 7f;
        [Tooltip("Angular Speed at Chasing State")]
        public float chaseAngSpeed = 240f;

        [Tooltip("Sight Angle when Chasing")]
        [Range(5f, 360f)]
        public float chaseSightAngle = 180f;


        /// <summary>
        /// Executed when state changed to Chase
        /// </summary>
        protected virtual void OnStateChangedToChase()
        {
            NavAgentMode = true;
            speed = chaseSpeed;
            angularSpeed = chaseAngSpeed;
            //NavTarget = WatchTarget;
            UpdateChasing();
        }

        /// <summary>
        /// Executed in every update if State is Chasing
        /// </summary>
        protected virtual void UpdateChasing()
        {
            if (WatchTarget == null)
                return;

            if (WatchTargetDistance < AttackDistance)
                State = NpcState.Attack;

            if ((lastUpdateTime + updateInterval) > Time.time)
                return;

            if (targetAtUpdate == null || WatchTarget.DistanceTo(targetAtUpdate.Value) < updateDistance) //did our target move enough
                return;

            MoveToTarget(WatchTarget.position);
        }
    }
}
