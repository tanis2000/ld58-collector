using UnityEngine.Audio;

namespace GameBase.Audio
{
    [System.Serializable]
    public class AudioTypeVolume
    {
        public AudioType AudioType = AudioType.Sound;
        public float Volume = 1.0f;
        // Default mixer group for this type of sound. It can be overridden per-cue.
        public AudioMixerGroup MixerGroup = null;
    };
}