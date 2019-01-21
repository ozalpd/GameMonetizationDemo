namespace Pops.Controllers.NPC
{
    public enum FightOrFleeResponse
    {
        /// <summary>
        /// Chases or Attacks to player or another NPC
        /// </summary>
        Fight = NpcState.Chasing,
        /// <summary>
        /// Retreat or escape from player or another NPC
        /// </summary>
        Flee = NpcState.Retreat,
        /// <summary>
        /// Keep same state
        /// </summary>
        KeepCalm = NpcState.Idle
    }

    public enum NpcState
    {
        Idle = 10,
        /// <summary>
        /// Patrolling or hanging around
        /// </summary>
        Patrol = 20,
        /// <summary>
        /// Chasing player or may be another NPC
        /// </summary>
        Chasing = 30,
        /// <summary>
        /// Attacking to player or may be to another NPC
        /// </summary>
        Attack = 40,
        /// <summary>
        /// Retreat or escape from a threat, may be from player or another NPC
        /// </summary>
        Retreat = 50
    }

    public enum WayPointPickMode
    {
        PickBySequence = 10,
        PickNearest = 20,
        PickRandom = 30
    }
}
