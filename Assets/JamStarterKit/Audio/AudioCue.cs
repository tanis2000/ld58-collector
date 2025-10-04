using System.Collections.Generic;
using GameBase.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace GameBase.Audio
{
    public class AudioCue : MonoBehaviour
    {
        public AudioType AudioType = AudioType.Sound;
        public bool Loop = false;
        public MinMaxRange Volume = new MinMaxRange(1.0f);
        public MinMaxRange Pitch = new MinMaxRange(1.0f);
        public MinMaxRange Pan = new MinMaxRange(0.0f);
        [Range(0, 1)] public float Chance = 1.0f;
        public AudioMixerGroup MixerGroup;
        public AudioReverbPreset ReverbPreset = AudioReverbPreset.Off;
        [Range(0, 1)] public float DistortionLevel = 0;
        public AudioEchoFilter EchoFilter;
        public AudioLowPassFilter LowPassFilter;
        public AudioHighPassFilter HighPassFilter;
        public AudioChorusFilter ChorusFilter;
        public List<Clip> Clips = new List<Clip>(1);

        public Clip GetClip()
        {
            if (Clips.Count == 0)
            {
                return null;
            }

            var rnd = AudioSystem.Instance().GetXRandom();
            return rnd.ItemWeighted(Clips, clip => clip.Weight);
        }
    }
}