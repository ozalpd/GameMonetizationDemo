using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Pops.Controllers.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        private AdsController adsController;

        [Header("HUD")]
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI livesText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI highScoreText;

        [Tooltip("Does not add labels like 'Score', 'Lives' etc. if set to true")]
        public bool displNumbersOnly;

        [Tooltip("Count down ticker for Boss Fight mode")]
        public TextMeshProUGUI bossTickerText;

        [Tooltip("Use only one of the sliders damage or health")]
        public Slider damageSlider;
        private Image _imgDamageFillArea;
        public Color damageMinColor = Color.yellow;
        public Color damageMaxColor = Color.red;

        [Tooltip("Use only one of the sliders damage or health")]
        public Slider healthSlider;
        private Image _imgHealthFillArea;
        public Color healthMaxColor = Color.green;
        public Color healthMidColor = Color.yellow;
        public Color healthMinColor = Color.red;

        public Slider progressSlider;
        private Image _imgProgressFillArea;
        public Color progressMinColor = Color.yellow;
        public Color progressMaxColor = Color.green;
        public TextMeshProUGUI progressText;

        [Header("Menu Items")]
        public Button extraLifeAdButton;
        public Button pauseButton;
        public Button resumeButton;
        public Image failureMenu;
        public Image pauseMenu;
        public Image successMenu;
        public TextMeshProUGUI gameStateText;

        [Header("Settings")]
        public Slider musicVolumeSlider;
        public Slider sFXVolumeSlider;

        [Header("Load Screen")]
        public GameObject loadScreen;
        public Slider loadSlider;

        /// <summary>
        /// TickerName of NumericTicker for count down of BossFight mode.
        /// </summary>
        public const string BossCountDown = "BossCountDown";


        public float MusicVolume
        {
            get { return AudioSettings.MusicVolume; }
            set { AudioSettings.MusicVolume = value; }
        }

        public float SfxVolume
        {
            get { return AudioSettings.SfxVolume; }
            set { AudioSettings.SfxVolume = value; }
        }

        private void Awake()
        {
            if (damageSlider != null)
                _imgDamageFillArea = damageSlider.fillRect.GetComponent<Image>();

            if (healthSlider != null)
                _imgHealthFillArea = healthSlider.fillRect.GetComponent<Image>();

            if (progressSlider != null)
                _imgProgressFillArea = progressSlider.fillRect.GetComponent<Image>();

            if (adsController == null)
                adsController = FindObjectOfType<AdsController>();
        }

        private void Start()
        {
            if (damageSlider != null)
            {
                if (damageMaxColor == damageMinColor)
                    _imgDamageFillArea.color = damageMaxColor;
                GameManager_DamageChanged(0, GameManager.MaxDamage);
                GameManager.DamageChanged += GameManager_DamageChanged;
            }

            if (healthSlider != null)
            {
                _imgHealthFillArea.color = healthMaxColor;
                GameManager_HealthChanged(GameManager.Health, GameManager.MaxHealth);
                GameManager.HealthChanged += GameManager_HealthChanged;
            }

            if (progressSlider != null)
            {
                if (progressMaxColor == progressMinColor)
                    _imgProgressFillArea.color = progressMaxColor;
                OnLevelProgressChanged(GameManager.LevelProgress, GameManager.MaxProgress);
                GameManager.LevelProgressChanged += OnLevelProgressChanged;
            }

            if (livesText != null)
            {
                GameManager_LivesChanged(GameManager.Lives);
                GameManager.LivesChanged += GameManager_LivesChanged;
            }

            if (scoreText != null)
            {
                GameManager_ScoreChanged(GameManager.Score);
                GameManager.ScoreChanged += GameManager_ScoreChanged;
            }

            if (highScoreText != null)
            {
                GameManager_HighScoreChanged(GameManager.HighScore);
                GameManager.HighScoreChanged += GameManager_HighScoreChanged;
            }

            if (bossTickerText != null)
            {
                NumericTicker.OnTickerTicked += NumericTicker_OnTickerTicked;
                NumericTicker.OnTickerCompleted += NumericTicker_OnTickerCompleted;
            }

            if (loadScreen != null)
                loadScreen.SetActive(false);

            GameManager.InvulnerabilityChanged += GameManager_InvulnerabilityChanged;

            //updateUI(GameManager.GameState);
            GameManager.GameStateChanged += UpdateUI;
            GameManager.GameState = GameState.Running;

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = MusicVolume;

            if (sFXVolumeSlider != null)
                sFXVolumeSlider.value = SfxVolume;
        }

        private void NumericTicker_OnTickerCompleted(NumericTicker ticker)
        {
            switch (ticker.TickerName)
            {
                case BossCountDown:
                    if (bossTickerText != null)
                        StartCoroutine(TurnOffCountDown(1.2f));
                    break;

                default:
                    break;
            }
        }

        private void NumericTicker_OnTickerTicked(NumericTicker ticker)
        {
            switch (ticker.TickerName)
            {
                case BossCountDown:
                    if (bossTickerText != null)
                    {
                        bossTickerText.gameObject.SetActive(true);
                        bossTickerText.text = string.Format("00:{0:00}", ticker.CurrentTick);
                    }
                    break;

                default:
                    break;
            }
        }

        private IEnumerator TurnOffCountDown(float blink)
        {
            if (bossTickerText != null)
            {
                yield return new WaitForSeconds(blink);
                bossTickerText.gameObject.SetActive(false);
                for (int i = 0; i < 5; i++)
                {
                    yield return new WaitForSeconds(blink * 0.5f);
                    bossTickerText.gameObject.SetActive(true);
                    yield return new WaitForSeconds(blink * 0.5f);
                    bossTickerText.gameObject.SetActive(false);
                }
            }
            else
            {
                yield return null;
            }
        }

        private void GameManager_HealthChanged(float health, float maxHealth)
        {
            healthSlider.value = health / maxHealth;
            if (healthSlider.value > 0.5f)
            {
                _imgHealthFillArea.color = healthMaxColor != healthMidColor
                                        ? Color.Lerp(healthMidColor, healthMaxColor, (healthSlider.value - 0.5f) * 2f)
                                        : healthMidColor;
            }
            else if (healthSlider.value < 0.5f)
            {
                _imgHealthFillArea.color = healthMinColor != healthMidColor
                                        ? Color.Lerp(healthMinColor, healthMidColor, healthSlider.value * 2f)
                                        : healthMidColor;
            }
            else
            {
                _imgHealthFillArea.color = healthMidColor;
            }

            if (healthText != null)
                healthText.text = health.ToString("00");
        }

        private void GameManager_InvulnerabilityChanged(bool invulnerable)
        {
            if (invulnerable)
                Debug.Log("Invulnerable!");
            else
                Debug.Log("Vulnerable!");

            //TODO: Connect this to UI
        }


        private void OnLevelProgressChanged(float progress, float maxValue)
        {
            progressSlider.value = progress / maxValue;
            if (progressMaxColor != progressMinColor)
                _imgProgressFillArea.color = Color.Lerp(progressMinColor, progressMaxColor, progress / maxValue);

            if (progressText != null)
                progressText.text = progress.ToString("00");
        }

        //Its better way to unsubscribe events inside from the GameManager
        //private void OnDestroy()
        //{
        //    GameManager.DamageChanged -= GameManager_DamageChanged;
        //    GameManager.HealthChanged -= GameManager_HealthChanged;
        //    GameManager.ProgressChanged -= GameManager_ProgressChanged;
        //    GameManager.LivesChanged -= GameManager_LivesChanged;
        //    GameManager.ScoreChanged -= GameManager_ScoreChanged;
        //    GameManager.HighScoreChanged -= GameManager_HighScoreChanged;
        //    GameManager.GameStateChanged -= updateUI;
        //}

        private void GameManager_DamageChanged(float damage, float maxDamage)
        {
            damageSlider.value = damage / maxDamage;
            if (damageMaxColor != damageMinColor)
                _imgDamageFillArea.color = Color.Lerp(damageMinColor, damageMaxColor, damage / maxDamage);
        }

        private void GameManager_LivesChanged(int lives)
        {
            livesText.text = displNumbersOnly
                            ? GameManager.Lives.ToString()
                            : string.Format("{0} {1}", GameManager.Lives, GameManager.Lives > 1 ? "LIVES" : "LIFE");
        }

        private void GameManager_ScoreChanged(int score)
        {
            scoreText.text = displNumbersOnly ? GameManager.Score.ToString() : string.Format("SCORE: {0}", GameManager.Score);
        }

        private void GameManager_HighScoreChanged(int score)
        {
            highScoreText.text = displNumbersOnly
                                ? GameManager.HighScore.ToString()
                                : string.Format("HIGH SCORE: {0}", GameManager.HighScore);
        }

        private IEnumerator InvulnerableFor(float duration)
        {
            GameManager.Invulnerable = true;
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + duration)
            {
                yield return null;
            }
            GameManager.Invulnerable = false;
        }

        public void MakeAnExtraLife()
        {
            if (adsController.CanShowRewardedAd)
                adsController.ShowExtraLifeAd(OnExtraLifeAdResult);
        }

        private void OnExtraLifeAdResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    GrantExtraLife();
                    break;

                case ShowResult.Skipped:
                    RestartLevel();
                    Debug.Log("The ad was skipped before reaching the end.");
                    break;

                case ShowResult.Failed:
                    if (adsController.HasRewardedAdShown)
                        GrantExtraLife();
                    else
                        Debug.LogError("The ad failed to be shown.");
                    break;
            }
        }

        private void GrantExtraLife()
        {
            GameManager.Lives++;
            StartCoroutine(InvulnerableFor(duration: 2.5f));
            StartCoroutine(ResumeAfter(duration: 1f));
            GameManager.Damage = 0;
        }

        public void PauseGame()
        {
            GameManager.GameState = GameState.Paused;
        }

        public void RestartLevel()
        {
            StartCoroutine(RestartLevelAsync());
        }

        private IEnumerator ResumeAfter(float duration)
        {
            UpdateUI(GameState.Running);

            //since TimeScale is zero this line will wait forever
            //yield return new WaitForSeconds(duration);

            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + duration)
            {
                yield return null;
            }

            GameManager.GameState = GameState.Running;
            yield return null;
        }

        public void ResumeGame()
        {
            StartCoroutine(ResumeAfter(duration: 0.5f));
        }

        public void DisplayLevels()
        {
            StartCoroutine(LoadSceneAsync("GameLevels"));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var asyncState = GameManager.LoadSceneAsync(sceneName);
            return ShowLoadState(asyncState);
        }

        private IEnumerator RestartLevelAsync()
        {
            var asyncState = GameManager.RestartLevelAsync();
            return ShowLoadState(asyncState);
        }

        private IEnumerator ShowLoadState(AsyncOperation loadState)
        {
            if (loadScreen != null)
                loadScreen.SetActive(true);

            if (pauseMenu != null)
                pauseMenu.gameObject.SetActive(false);

            if (failureMenu != null)
                failureMenu.gameObject.SetActive(false);

            if (successMenu != null)
                successMenu.gameObject.SetActive(false);

            while (!loadState.isDone)
            {
                //float progress = Mathf.Clamp01(loadState.progress / 0.9f);
                if (loadSlider != null)
                    loadSlider.value = Mathf.Clamp01(loadState.progress / 0.9f);

                yield return null;
            }
        }

        private void UpdateUI(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Succeeded:
                    if (gameStateText != null)
                        gameStateText.text = "THIS LEVEL SUCCESSFULLY\r\nCOMPLETED";
                    break;

                case GameState.Failed:
                    if (gameStateText != null)
                        gameStateText.text = "GAME OVER";
                    break;

                case GameState.Paused:
                    if (gameStateText != null)
                        gameStateText.text = "GAME PAUSED";
                    break;

                case GameState.Running:
                    break;

                default:
                    break;
            }

            //TODO: Do sth for GameState.NotStarted

            if (pauseButton != null)
                pauseButton.gameObject.SetActive(gameState == GameState.Running);
            if (resumeButton != null)
                resumeButton.gameObject.SetActive(gameState == GameState.Paused);
            if (pauseMenu != null)
                pauseMenu.gameObject.SetActive(gameState == GameState.Paused);
            if (failureMenu != null)
                failureMenu.gameObject.SetActive(gameState == GameState.Failed);
            if (extraLifeAdButton != null)
                extraLifeAdButton.gameObject.SetActive(adsController != null && adsController.CanShowExtraLifeAd);

            if (successMenu != null)
                successMenu.gameObject.SetActive(gameState == GameState.Succeeded);
        }
    }
}