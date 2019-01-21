using UnityEngine;
using UnityEngine.Audio;

namespace Pops.Controllers
{
    public class AudioController : MonoBehaviour
    {
        public AudioMixer mixer;

        private void Start()
        {
            setMixerVolume("VolumeMusic", GameSettings.MusicVolume);
            setMixerVolume("VolumeSFX", GameSettings.SfxVolume);

            GameSettings.MusicVolumeChanged += MusicVolumeChanged;
            GameSettings.SfxVolumeChanged += SfxVolumeChanged;
        }

        private void OnDestroy()
        {
            GameSettings.SfxVolumeChanged -= SfxVolumeChanged;
            GameSettings.MusicVolumeChanged -= MusicVolumeChanged;
        }

        private void MusicVolumeChanged(float volume)
        {
            setMixerVolume("VolumeMusic", volume);
        }

        private void SfxVolumeChanged(float volume)
        {
            setMixerVolume("VolumeSFX", volume);
        }


        private void setMixerVolume(string mixerName, float volume)
        {
            if (!(volume > 0))
                volume = 0.0000025f;
            float dbVol = 20 * Mathf.Log10(volume);
            mixer.SetFloat(mixerName, dbVol);
        }
    }
}