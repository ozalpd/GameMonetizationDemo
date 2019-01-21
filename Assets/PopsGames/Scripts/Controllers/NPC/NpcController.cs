using Pops.Controllers;
using Pops.Extensions;
using Pops.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//partial class file for
//any State and parameters
namespace Pops.Controllers.NPC
{
    /// <summary>
    /// Abstract NPC Controller class
    /// </summary>
    public abstract partial class NpcController : BaseCharacterController
    {
        [SerializeField]
        private NpcState state = NpcState.Idle;

        [Header("Sight Settings")]
        [SerializeField]
        [Tooltip("Watch target. When this is null, NPC does not try to see on updates.")]
        private Transform watchTarget;

        [SerializeField]
        [Tooltip("Response on Visual Contact with Watch Target")]
        private FightOrFleeResponse visContResponse = FightOrFleeResponse.Fight;

        [Tooltip("When calculating visibilty of any target, that value is added to transform's Y component.")]
        [Range(0f, 10f)]
        public float sightHeight = 1f;

        [Tooltip("Game Object tags which prevent having visual contact with Watch Target")]
        public string[] sightObstacleTags;

        [Tooltip("Visible Distance")]
        [Range(5f, 100f)]
        public float visibleDist = 20f;
        [Tooltip("Far Sight Angle")]
        [Range(5f, 360f)]
        public float farSightAngle = 60f;

        [Tooltip("Near Sight begining distance. When target gets closer than this distance sight angle increases.")]
        [Range(1f, 30f)]
        public float nearSightDist = 8f;
        [Tooltip("Near Sight Angle. This is maximum sight angle if distance to target will be zero.")]
        [Range(5f, 360f)]
        public float nearSightAngle = 180f;

        [Header("NavAgent")]
        [Tooltip("Navigation Target. When set to null, stops updating NPC's destination.")]
        private Transform navTarget;

        [Header("NavAgent when Chasing or Retreating")]
        [Tooltip("Update Interval for navigation path calculations. Smaller values need more CPU power.")]
        public float updateInterval = 0.5f;

        [Tooltip("Minimum distance between target's current position and last position at update time. Smaller values need more CPU power.")]
        public float updateDistance = 0.5f;

        private float lastUpdateTime = 0;
        private float speed = 2f;

        //Target's last position at update time.
        private Vector3? targetAtUpdate;

        private float angularSpeed;
        private float rotationSpeed;

        protected NavMeshObstacle obstacle;
        protected NavMeshAgent agent;
        protected float fleeRadius = 10f;

        protected override void Awake()
        {
            base.Awake();

            agent = GetComponent<NavMeshAgent>();
            obstacle = GetComponent<NavMeshObstacle>();
            if (agent != null && obstacle != null)
            {
                obstacle.enabled = !agent.enabled;
            }

            OnStateChanged();
            rotationSpeed = 0f;
            speed = 0;
        }

        protected virtual void Update()
        {
            switch (State)
            {
                case NpcState.Idle:
                    break;

                case NpcState.Patrol:
                    UpdatePatrolling();
                    break;

                case NpcState.Chasing:
                    UpdateChasing();
                    break;

                case NpcState.Attack:
                    UpdateAttacking();
                    break;

                case NpcState.Retreat:
                    UpdateRetreating();
                    break;

                default:
                    break;
            }

            IsWatchTargetVisible = WatchTarget != null && IsTargetVisible(WatchTarget);
        }

        /// <summary>
        /// Index of current (target) way point
        /// </summary>
        private int wpIndex;

        /// <summary>
        /// Gets or sets the current waypoint.
        /// </summary>
        /// <value>The current wp.</value>
        public virtual Transform TargetWP
        {
            get { return _currentWP; }
            set
            {
                BeforePrevWP = PreviousWP;
                PreviousWP = _currentWP;
                _currentWP = value;
                wpIndex = Array.IndexOf(waypoints, _currentWP);
                NavTarget = _currentWP;
            }
        }
        private Transform _currentWP;


        /// <summary>
        /// Gets a value indicating whether NPC has visual contact to WatchTarget or not.
        /// </summary>
        /// <value><c>true</c> if has visual contact; otherwise, <c>false</c>.</value>
        public bool IsWatchTargetVisible
        {
            get { return _watchTargetVisible; }
            protected set
            {
                if (_watchTargetVisible != value)
                {
                    _watchTargetVisible = value;
                    if (_watchTargetVisible)
                        OnHavingVisualContact(WatchTarget);
                    else
                        OnLosingVisualContact(WatchTarget);
                }
            }
        }
        private bool _watchTargetVisible;

        protected virtual void OnHavingVisualContact(Transform target)
        {
            stateBeforeVisContact = State;

            if (VisualContactResponse == FightOrFleeResponse.Fight)
            {
                State = NpcState.Chasing;
            }
            else if (VisualContactResponse == FightOrFleeResponse.Flee)
            {
                State = NpcState.Retreat;
            }
        }
        private NpcState stateBeforeVisContact;

        protected virtual void OnLosingVisualContact(Transform target)
        {
            State = stateBeforeVisContact;
        }

        /// <summary>
        /// Enables/disables NavMeshAgent component. When set true disables NavMeshObstacle component and when set false enables NavMeshObstacle component.
        /// </summary>
        public bool NavAgentMode
        {
            get
            {
                return agent != null && agent.enabled;
            }
            set
            {
                if (agent != null && value)
                {
                    if (obstacle != null)
                        obstacle.enabled = false;

                    agent.enabled = true;
                }
                else if (agent != null)
                {
                    agent.enabled = false;

                    if (obstacle != null)
                        obstacle.enabled = true;
                }
                else if (obstacle != null)
                {
                    obstacle.enabled = !value;
                }
            }
        }

        /// <summary>
        /// Enables/disables NavMeshObstacle component. When set true disables NavMeshAgent component and when set false enables NavMeshAgent component.
        /// </summary>
        public bool NavObstacleMode
        {
            get
            {
                return obstacle != null && obstacle.enabled;
            }
            set
            {
                if (obstacle != null && value)
                {
                    if (agent != null)
                        agent.enabled = false;

                    obstacle.enabled = true;
                }
                else if (obstacle != null)
                {
                    obstacle.enabled = false;

                    if (agent != null)
                        agent.enabled = true;
                }
                else if (agent != null)
                {
                    agent.enabled = !value;
                }
            }
        }

        /// <summary>
        /// Navigation Target. When set to null, stops updating NPC's destination.
        /// </summary>
        /// <value>The target.</value>
        public virtual Transform NavTarget
        {
            get { return navTarget; }
            set
            {
                navTarget = value;
                UpdateDestination();
            }
        }

        /// <summary>
        /// Watch target. When this is null, NPC does not try to see on frame updates.
        /// </summary>
        public virtual Transform WatchTarget
        {
            get { return watchTarget; }
            set
            {
                if (watchTarget != value)
                {
                    watchTarget = value;
                }
            }
        }

        /// <summary>
        /// Distance to WatchTarget
        /// </summary>
        public float WatchTargetDistance
        {
            get
            {
                if (WatchTarget == null)
                    return float.MaxValue;

                return WatchTarget.DistanceTo(transform.position, true);
            }
        }

        /// <summary>
        /// Transform's position that added sightHeight to its Y component
        /// </summary>
        protected Vector3 SightPosition
        {
            get
            {
                return transform.position.With(y: transform.position.y + sightHeight);
            }
        }

        public NpcState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    OnStateChanged();
                }
            }
        }

        protected virtual void OnStateChanged()
        {
            switch (State)
            {
                case NpcState.Idle:
                    OnStateChangedToIdle();
                    break;

                case NpcState.Patrol:
                    OnStateChangedToPatrol();
                    break;

                case NpcState.Chasing:
                    OnStateChangedToChase();
                    break;

                case NpcState.Attack:
                    OnStateChangedToAttack();
                    break;

                case NpcState.Retreat:
                    OnStateChangedToRetreat();
                    break;
            }

            if (agent != null)
            {
                agent.speed = speed;
                agent.angularSpeed = angularSpeed;
            }
        }

        /// <summary>
        /// Executed when state changed to Idle
        /// </summary>
        protected virtual void OnStateChangedToIdle()
        {
            NavObstacleMode = true;
        }

        public float DistanceTo(Vector3 destination)
        {
            return transform.DistanceTo(destination);
        }

        public FightOrFleeResponse VisualContactResponse
        {
            get { return visContResponse; }
            set
            {
                if (visContResponse != value)
                {
                    visContResponse = value;
                    OnVisualContactResponseChanged();
                }
            }
        }

        protected virtual void OnVisualContactResponseChanged()
        {
            switch (VisualContactResponse)
            {
                case FightOrFleeResponse.Fight:
                    if (State == NpcState.Retreat)
                        State = NpcState.Chasing;
                    break;

                case FightOrFleeResponse.Flee:
                    if (State == NpcState.Chasing || State == NpcState.Attack)
                        State = NpcState.Retreat;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Detects there is any sight obstacle between NPC and the target.
        /// </summary>
        /// <returns><c>true</c>, if sight obstacle between target was anyed, <c>false</c> otherwise.</returns>
        /// <param name="target">Target.</param>
        public virtual bool AnySightObstacleBetweenTarget(Transform target)
        {
            if (sightObstacleTags == null || sightObstacleTags.Length < 1)
                return false;

            ray = new Ray(SightPosition, target.position);
            if (Physics.Raycast(ray, out hit, DistanceTo(target.position)))
            {
                return sightObstacleTags.Contains(hit.collider.tag);
            }

            return false;
        }
        private Ray ray;
        private RaycastHit hit;

        /// <summary>
        /// Detects that the target is visible.
        /// </summary>
        /// <returns><c>true</c>, if target visible, <c>false</c> otherwise.</returns>
        /// <param name="target">Target transform</param>
        public virtual bool IsTargetVisible(Transform target)
        {
            if (AnySightObstacleBetweenTarget(target))
                return false;

            visDist = State == NpcState.Retreat ? retreatVisDist : visibleDist;
            targetDist = DistanceTo(target.position);
            if (targetDist > visDist)
                return false;

            if (State == NpcState.Chasing)
            {
                targetAngle = chaseSightAngle * 0.5f;
            }
            else if (State == NpcState.Retreat)
            {
                targetAngle = retreatSightAngle * 0.5f;
            }
            else if (targetDist < 0.1)
            {
                targetAngle = nearSightAngle * 0.5f;
            }
            else if (targetDist < nearSightDist)
            {
                targetAngle = Mathf.Lerp(nearSightAngle, farSightAngle, targetDist / nearSightDist) * 0.5f;
            }
            else
            {
                targetAngle = farSightAngle * 0.5f;
            }

            if (targetAngle < 179.9f)
                return Vector3.Angle((target.position.With(y: transform.position.y) - transform.position),
                                     transform.forward) < targetAngle;
            else  //we don't need to make any calculation for sight angle of 2*180
                return true;
        }
        private float targetDist;
        private float targetAngle;
        private float visDist;

        /// <summary>
        /// Flees from given point.
        /// </summary>
        /// <param name="thePoint">The point from to flee</param>
        /// <param name="fleeDistance">Flee distance.</param>
        protected bool FleeFromPoint(Vector3 thePoint, float fleeDistance)
        {
            fleeDirection = (transform.position - thePoint).normalized;
            fleeGoal = transform.position + (fleeDirection * fleeDistance);
            NavAgentMode = true;

            fleePath = new NavMeshPath();
            agent.CalculatePath(fleeGoal, fleePath);

            if (fleePath.status != NavMeshPathStatus.PathInvalid)
            {
                //Debug.Log("Flee to opposite position at " + fleeDistance);
                agent.SetDestination(fleePath.corners[fleePath.corners.Length - 1]);
                isFleeing = true;
            }
            else if (FindRndPoint(fleeGoal, 1f, out fleeGoal))
            {
                //fleeGoal = navHit.position;
                //Debug.Log("Found a random position to flee at " + fleeDistance);
                agent.SetDestination(fleeGoal);
                isFleeing = true;
            }
            else if (fleeDistance < fleeRadius * 0.1f) //descriptions are at else block
            {
                int i = 0;
                //try to find a random fleeDirection
                do
                {
                    fleeDirection = (UnityEngine.Random.insideUnitSphere * fleeDistance * 0.5f) + transform.position;
                    isFleeing = NavMesh.SamplePosition(fleeDirection, out navHit, fleeDistance, 1);
                    i++; //don't push too hard
                } while (!isFleeing || i < 30);

                Debug.Log("Searched for a random fleeDirection at " + fleeDistance + ", isFleeing " + isFleeing);

                if (isFleeing)
                    agent.SetDestination(navHit.position);
            }
            else
            {   //We are trying half distance.
                FleeFromPoint(thePoint, fleeDistance * 0.5f);
                //At fourth time fleeDistance will be fleeRadius * 0.125f
                //At fifth previous block will run
            }

            return isFleeing;
        }
        private NavMeshHit navHit;
        private Vector3 fleeDirection;
        private Vector3 fleeGoal;
        private NavMeshPath fleePath;
        private bool isFleeing;

        /// <summary>
        /// Finds a random point for NPC
        /// </summary>
        /// <returns><c>true</c>, if random point was found, <c>false</c> otherwise.</returns>
        /// <param name="center">Center.</param>
        /// <param name="range">Range.</param>
        /// <param name="result">Result.</param>
        /// <param name="areaMask">Area mask.</param>
        /// <param name="flatten">If set to <c>true</c> sets y component of the random point same as NPC's</param>
        protected bool FindRndPoint(Vector3 center, float range, out Vector3 result, int areaMask = NavMesh.AllAreas, bool flatten = true)
        {
            for (int i = 0; i < 30; i++)
            {
                randomPoint = (center + UnityEngine.Random.insideUnitSphere * range);
                if (flatten)
                    randomPoint = randomPoint.With(y: transform.position.y);

                if (NavMesh.SamplePosition(randomPoint, out meshHit, 1.0f, areaMask))
                {
                    result = meshHit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
        private NavMeshHit meshHit;
        private Vector3 randomPoint;

        protected virtual void LookToward(Vector3 targetPos)
        {
            lookDirection = targetPos - transform.position;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                          Quaternion.LookRotation(lookDirection),
                                                          rotationSpeed * Time.deltaTime);
            }
        }
        private Vector3 lookDirection;

        protected virtual void MoveToTarget(Vector3 targetPos)
        {
            if (NavAgentMode)
            {
                agent.SetDestination(targetPos);
            }
            else
            {
                MoveToTargetByTranslate(targetPos);
            }
        }

        protected virtual void MoveToTargetByTranslate(Vector3 targetPos)
        {
            LookToward(targetPos);
            transform.Translate(0, 0, speed * Time.deltaTime);
        }

        protected virtual void UpdateDestination()
        {
            if (NavTarget == null)
            {
                targetAtUpdate = null;
            }
            else
            {
                MoveToTarget(NavTarget.position);
                targetAtUpdate = NavTarget.position;
            }

            lastUpdateTime = Time.time;
        }
    }
}