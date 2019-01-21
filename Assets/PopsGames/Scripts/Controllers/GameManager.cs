using Pops.GameObjects.Tools;
using Pops.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;


namespace Pops.Controllers
{
    /// <summary>
    /// Static game controller class not complicated enough to be called as 'Manager'
    /// </summary>
    /// <remarks>
    /// Do not use SceneManager.LoadScene directly, instead use GameController's LoadScene methods
    /// </remarks>
    public static class GameManager
    {
        public static float MaxDamage = 100;
        public static float MaxHealth = 100;
        public static float MaxProgress = 100;

        public const int StartingLives = 1;

        public delegate void GameStateChange(GameState gameState);

        public delegate void ProgressChange(float progress, float maxValue);
        public delegate void LivesChange(int lives);
        public delegate void ScoreChange(int score);

        public delegate void PlayerMove(AbstractPlayerController player, Vector3 movement);

        public static GameState GameState
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    if (GameStateChanged != null)
                        GameStateChanged(_state);

                    switch (_state)
                    {
                        case GameState.NotStarted:
                            Debug.LogWarning("This is default state and does not intended to set from another GameState!");
                            break;

                        case GameState.Paused:
                            //TODO:Set UserControls disabled
                            Time.timeScale = 0;
                            break;

                        case GameState.Running:
                            //TODO:Set UserControls enabled
                            Time.timeScale = 1;
                            break;

                        case GameState.Succeeded:
                            //TODO:Set UserControls disabled
                            Time.timeScale = 0;
                            break;

                        case GameState.Failed:
                            //TODO:Set UserControls disabled
                            Time.timeScale = 0;
                            break;
                    }
                }
            }
        }
        private static GameState _state;
        public static event GameStateChange GameStateChanged;


        public static float Damage
        {
            get { return _damage; }
            set
            {
                if (!Mathf.Approximately(_damage, value))
                {
                    _damage = value < MaxDamage ? value > 0 ? value : 0f : MaxDamage;
                    if (Mathf.Approximately(_damage, MaxDamage))
                    {
                        Lives--;
                        if (GameState != GameState.Failed)
                        {
                            _damage = 0;
                            Health = MaxHealth;
                        }
                    }
                    if (DamageChanged != null)
                        DamageChanged(_damage, MaxDamage);
                }
            }
        }
        private static float _damage;
        public static event ProgressChange DamageChanged;


        public static float Health
        {
            get
            {
                if (_health == null)
                    _health = MaxHealth;
                return _health.Value;
            }
            set
            {
                if (!Mathf.Approximately(_health.Value, value))
                {
                    _health = value < MaxHealth ? value > 0 ? value : 0f : MaxHealth;
                    if (Mathf.Approximately(_health.Value, 0))
                    {
                        Lives--;
                        if (GameState != GameState.Failed)
                        {
                            _health = MaxHealth;
                            Damage = 0;
                        }
                    }

                    if (HealthChanged != null)
                        HealthChanged(_health.Value, MaxHealth);
                }
            }
        }
        private static float? _health;
        public static event ProgressChange HealthChanged;



        public static float LevelProgress
        {
            get
            {
                if (_levelProgress == null)
                    _levelProgress = 0f;
                return _levelProgress.Value;
            }
            set
            {
                if (!Mathf.Approximately(_levelProgress.Value, value))
                {
                    _levelProgress = value < MaxProgress ? value > 0 ? value : 0f : MaxProgress;
                    if (LevelProgressChanged != null)
                        LevelProgressChanged(_levelProgress.Value, MaxProgress);
                }
            }
        }
        private static float? _levelProgress;
        public static event ProgressChange LevelProgressChanged;


        public static int Lives
        {
            get
            {
                if (_lives == null)
                    _lives = StartingLives;
                return _lives.Value;
            }
            set
            {
                if (_lives != value)
                {
                    _lives = value;

                    if (LivesChanged != null)
                        LivesChanged(_lives.Value);

                    if (_lives <= 0)
                        GameState = GameState.Failed;
                }
            }
        }
        private static int? _lives;
        public static event LivesChange LivesChanged;


        public static int Score
        {
            get { return _score; }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    HighScore = _score;
                    if (ScoreChanged != null)
                        ScoreChanged(_score);
                }
            }
        }
        private static int _score;
        public static event ScoreChange ScoreChanged;


        public static int HighScore
        {
            get
            {
                if (_highScore == null)
                {
                    _highScore = PlayerPrefs.GetInt("HighScore", 0);
                }
                return _highScore.Value;
            }
            set
            {
                if (value > (_highScore ?? 0))
                {
                    _highScore = value;
                    PlayerPrefs.SetInt("HighScore", _highScore.Value);

                    if (HighScoreChanged != null)
                        HighScoreChanged(_highScore.Value);
                }
            }
        }
        private static int? _highScore;
        public static event ScoreChange HighScoreChanged;


        /// <summary>
        /// Loads the Scene by its index in Build Settings or by its name.
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            ReleaseSubscribers();
            ResetMetrics();
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }

        /// <summary>
        /// Loads the Scene by its index in Build Settings or by its name.
        /// </summary>
        /// <param name="sceneName">Name or path of the Scene to load.</param>
        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            ReleaseSubscribers();
            ResetMetrics();
            SceneManager.LoadScene(sceneName, mode);
        }

        /// <summary>
        /// Loads the Scene asynchronously in the background by its index in Build Settings or by its name.
        /// </summary>
        /// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
        /// <returns></returns>
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            ReleaseSubscribers();
            ResetMetrics();
            return SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
        }

        /// <summary>
        /// Loads the Scene asynchronously in the background by its index in Build Settings or by its name.
        /// </summary>
        /// <param name="sceneName">Name or path of the Scene to load.</param>
        /// <returns></returns>
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            ReleaseSubscribers();
            ResetMetrics();
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        public static void PlayerMoving(AbstractPlayerController player, Vector3 movement)
        {
            if (OnPlayerMoving != null)
                OnPlayerMoving(player, movement);
        }
        public static event PlayerMove OnPlayerMoving;
        //public static event PlayerMove OnPlayerMoved;//

        public static void PlayerStopped(AbstractPlayerController player)
        {
            if (OnPlayerStopped != null)
                OnPlayerStopped(player, Vector3.zero);
        }
        public static event PlayerMove OnPlayerStopped;


        public static void RestartLevel()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            LoadScene(sceneIndex);
        }

        public static AsyncOperation RestartLevelAsync()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            var asyncState = SceneManager.LoadSceneAsync(sceneIndex);

            ReleaseSubscribers();
            ResetMetrics();
            return asyncState;
        }

        private static void ReleaseSubscribers()
        {
            GameStateChanged = null;
            DamageChanged = null;
            HealthChanged = null;
            HighScoreChanged = null;
            LivesChanged = null;
            //OnPlayerMoving = null;
            ScoreChanged = null;
        }

        private static void ResetMetrics()
        {
            _damage = 0;
            _health = null;
            _lives = null;
            _score = 0;
            _state = GameState.NotStarted;
            _levelProgress = null;
            NumericTicker.DisposeAllTickers();
        }

        private static void RestartLevelByObjectPool()
        {
            //ObjectPool.ClearAllPools();

            //TODO: Find objects which should be reset

            var savedTransforms = Object.FindObjectsOfType<SaveTransform>();
            foreach (var t in savedTransforms)
            {
                if (t.IsSaved)
                    t.RestoreTransform();
            }

            ResetMetrics();
        }
    }
}