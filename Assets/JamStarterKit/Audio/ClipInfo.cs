using UnityEngine;

namespace GameBase.Audio
{
    public class ClipInfo
    {
        //ClipInfo used to maintain default audio source info
        public AudioCue Cue;
        public AudioHandle Handle;
        public AudioType AudioType;
        public float DefaultVolume;
        public float DefaultPitch;
        public float TargetVolume; // targetVolume used for fading in/out (especially when handling save/load)
        public float FadeDelta; // volume change per second
        public bool StopAfterFade; // Whether to stop after fadeout
        public Transform Emitter;
        public bool Paused;
    }
}