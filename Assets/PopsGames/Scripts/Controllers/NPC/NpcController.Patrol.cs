using Pops.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//For Patrolling State
namespace Pops.Controllers.NPC
{
    public abstract partial class NpcController
    {
        [Header("Patrol State Settings")]
        [Tooltip("Way points for NPC patrolling")]
        public Transform[] waypoints;

        [Tooltip("The way to pick first WP when state changed to Patrolling.")]
        public WayPointPickMode pickFirstWP = WayPointPickMode.PickBySequence;

        [Tooltip("Accuracy for arriving current (target) way point")]
        public float accuracy = 2f;

        [Range(0f, 1f)]
        [Tooltip("Shortcut usage rate between way points. One is always, zero never.")]
        public float randShortcutRate = 0.2f;

        [Tooltip("Patrolling or hanging around speed")]
        public float patrollingSpeed = 3.5f;     //default speed of NavMeshAgent
        [Tooltip("Angular Speed at Patrolling State")]
        public float patrollingAngSpeed = 120f;  //default speed of NavMeshAgent

        [Tooltip("Time span that NPC stays in Patrolling state. If value is zero, NPC keeps Patrolling state, otherwise after the duration state will be set to Idle.")]
        [Range(0f, 600f)]
        public float patrollingDuration = 0f;
        private float patrollingStartTime = -1f;

        /// <summary>
        /// Executed when state changed to Patrol
        /// </summary>
        protected virtual void OnStateChangedToPatrol()
        {
            NavAgentMode = true;
            speed = patrollingSpeed;
            angularSpeed = patrollingAngSpeed;

            switch (pickFirstWP)
            {
                case WayPointPickMode.PickBySequence:
                    if (TargetWP == null)
                        TargetWP = waypoints.FirstOrDefault();
                    else
                        NavTarget = TargetWP;
                    break;

                case WayPointPickMode.PickNearest:
                    PickNearestWP();
                    break;

                case WayPointPickMode.PickRandom:
                    PickRandomWP();
                    break;

                default:
                    break;
            }

            if (patrollingDuration > 0)
                patrollingStartTime = Time.time;
        }

        protected virtual void UpdatePatrolling()
        {
            if (patrollingDuration > 0 &&patrollingStartTime > 0 && Time.time > patrollingStartTime + patrollingDuration)
            {
                State = NpcState.Idle;
                patrollingStartTime = -1f;
                return;
            }

            if (waypoints.Length < 1 || State != NpcState.Patrol)
                return;

            if (transform.DistanceTo(waypoints[wpIndex].position) < accuracy)
            {
                if (randShortcutRate < 1 && randShortcutRate > UnityEngine.Random.Range(0f, 1f))
                {
                    PickNearestWP();
                }
                else
                {
                    wpIndex++;
                    if (!(wpIndex < waypoints.Length))
                        wpIndex = 0;

                    NavTarget = waypoints[wpIndex];
                }
            }
        }

        protected virtual void PickRandomWP()
        {
            filterdWPs = waypoints
                .Where(g => g != TargetWP && g != PreviousWP && g != BeforePrevWP)
                .ToList();

            tmpWP = filterdWPs[UnityEngine.Random.Range(0, filterdWPs.Count - 1)];
            TargetWP = tmpWP;
        }
        private List<Transform> filterdWPs;

        protected void PickNearestWP()
        {
            orderedWPs = waypoints
                .Where(g => g != TargetWP && g != PreviousWP && g != BeforePrevWP)
                .OrderBy(g => g.DistanceTo(transform.position));

            tmpWP = orderedWPs.FirstOrDefault();
            TargetWP = tmpWP;
        }
        private IOrderedEnumerable<Transform> orderedWPs;
        private Transform tmpWP; //To keep next prevWP until query to be executed;
        protected Transform PreviousWP { get; private set; }
        protected Transform BeforePrevWP { get; private set; }
    }
}
