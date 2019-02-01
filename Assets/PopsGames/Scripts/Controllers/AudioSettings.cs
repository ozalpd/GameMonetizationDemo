using UnityEngine;

namespace Pops.Controllers
{
    /// <summary>
    /// Audio settings, esp events for GameUI.
    /// </summary>
    public static class AudioSettings
    {
        public delegate void VolumeChange(float volume);

        private const string VolMusic = AudioController.VolMusic;
        private const string VolSFX = AudioController.VolSFX;

        public static float MusicVolume
        {
            get
            {
                if (_musicVolume == null)
                    _musicVolume = PlayerPrefs.GetFloat(VolMusic, 0.8f);
                return _musicVolume.Value;
            }
            set
            {
                if (!_musicVolume.HasValue || !Mathf.Approximately(_musicVolume.Value, value))
                {
                    _musicVolume = value;
                    PlayerPrefs.SetFloat(VolMusic, _musicVolume.Value);

                    if (MusicVolumeChanged != null)
                        MusicVolumeChanged(_musicVolume.Value);
                }
            }
        }
        private static float? _musicVolume;
        public static event VolumeChange MusicVolumeChanged;


        public static float SfxVolume
        {
            get
            {
                if (_sfxVolume == null)
                    _sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
                return _sfxVolume.Value;
            }
            set
            {
                if (!_sfxVolume.HasValue || !Mathf.Approximately(_sfxVolume.Value, value))
                {
                    _sfxVolume = value;
                    PlayerPrefs.SetFloat("SfxVolume", _sfxVolume.Value);

                    if (SfxVolumeChanged != null)
                        SfxVolumeChanged(_sfxVolume.Value);
                }
            }
        }
        private static float? _sfxVolume;
        public static event VolumeChange SfxVolumeChanged;
    }
}