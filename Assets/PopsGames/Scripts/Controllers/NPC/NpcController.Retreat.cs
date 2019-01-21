using Pops.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//For Retreating State
namespace Pops.Controllers.NPC
{
    public abstract partial class NpcController
    {
        [Header("Retreat State Settings")]
        [Tooltip("Retreating or escaping speed")]
        public float retreatSpeed = 7f;

        [Tooltip("Angular Speed at Retreating State")]
        public float retreatAngSpeed = 240f;

        [Tooltip("Visible Distance when Retreating")]
        [Range(5f, 100f)]
        public float retreatVisDist = 40f;

        [Tooltip("Sight Angle when Retreating")]
        [Range(5f, 360f)]
        public float retreatSightAngle = 360f;



        /// <summary>
        /// Executed when state changed to Retreat
        /// </summary>
        protected virtual void OnStateChangedToRetreat()
        {

            NavAgentMode = true;
            speed = retreatSpeed;
            angularSpeed = retreatAngSpeed;
            FleeFromPoint(watchTarget.position, fleeRadius);
        }

        protected virtual void UpdateRetreating()
        {
            //if (WatchTarget == null)
            //    return;

            //if ((lastUpdateTime + updateInterval) > Time.time)
            //    return;

            //if (targetAtUpdate == null || WatchTarget.DistanceTo(targetAtUpdate.Value) < updateDistance) //did our target move enough
            //return;
            if ((agent.pathEndPosition - transform.position).magnitude < accuracy)
                FleeFromPoint(watchTarget.position, fleeRadius);
        }
    }
}
