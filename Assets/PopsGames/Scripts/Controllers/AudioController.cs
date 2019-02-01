using UnityEngine;
using UnityEngine.Audio;

namespace Pops.Controllers
{
    public class AudioController : MonoBehaviour
    {
        public AudioMixer mixer;

        [SerializeField]
        private AudioSource bGMusic;

        public const string VolMusic = "VolumeMusic";
        public const string VolSFX = "VolumeSFX";

        private float minVolume = 0.0000025f;

        public void Awake()
        {
            if (bGMusic == null)
                bGMusic = GetComponent<AudioSource>();
            if (bGMusic != null)
                bGMusic.loop = true;
        }

        private void Start()
        {
            setMixerVolume(VolMusic, AudioSettings.MusicVolume);
            setMixerVolume(VolSFX, AudioSettings.SfxVolume);

            AudioSettings.MusicVolumeChanged += Settings_MusicVolumeChanged;
            AudioSettings.SfxVolumeChanged += Settings_SfxVolumeChanged;

            GameManager.GameStateChanged += GameManager_StateChanged;
        }

        private void OnDestroy()
        {
            AudioSettings.SfxVolumeChanged -= Settings_SfxVolumeChanged;
            AudioSettings.MusicVolumeChanged -= Settings_MusicVolumeChanged;
            GameManager.GameStateChanged -= GameManager_StateChanged;
        }

        private void GameManager_StateChanged(GameState gameState)
        {
            bool pauseMusic = false;
            switch (gameState)
            {
                case GameState.Running:
                    break;
                case GameState.Paused:
                    pauseMusic = true;
                    break;
                case GameState.Failed:
                    //TODO: play fail music
                    break;
                case GameState.Succeeded:
                    //TODO: play success music
                    break;
                default:
                    break;
            }
            if (pauseMusic && bGMusic != null)
            {
                bGMusic.Pause();
            }
            else if (AudioSettings.MusicVolume > minVolume && bGMusic != null)
            {
                bGMusic.UnPause();
            }
        }

        private void Settings_MusicVolumeChanged(float volume)
        {
            setMixerVolume(VolMusic, volume);
        }

        private void Settings_SfxVolumeChanged(float volume)
        {
            setMixerVolume(VolSFX, volume);
        }


        private void setMixerVolume(string mixerName, float volume)
        {
            if (volume < minVolume)
            {
                volume = minVolume;
                if (bGMusic != null && mixerName.Equals(VolMusic))
                    bGMusic.Pause();
            }
            else if (volume > minVolume && bGMusic != null && mixerName.Equals(VolMusic))
            {
                bGMusic.UnPause();
            }
            float dbVol = 20 * Mathf.Log10(volume);
            mixer.SetFloat(mixerName, dbVol);
        }
    }
}