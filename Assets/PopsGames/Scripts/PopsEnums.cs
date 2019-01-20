namespace Pops
{
    /// <summary>
    /// Possible states of the application
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Game or level was just loaded and is not started yet
        /// </summary>
        NotStarted = 0,
        Running = 10,
        Paused = 20,
        Failed = 30,
        Succeeded = 40
    }

    public enum MoveMechanism
    {
        SetPositionAndAccelerate = 0,
        SetPosition = 10,
        SetVelocity = 20,
        AddForce = 30
    }

    public enum SpawnPointChoice
    {
        Randomly = 10,
        Sequentally = 20,
        AwayFromPlayer = 30,
        CloseToPlayer = 40
    }

    public enum TickerState
    {
        Active = 1,
        Paused = 2,
        Completed = 3
    }
}
