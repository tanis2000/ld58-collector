using UnityEngine;

namespace GameBase.Audio
{
    public class AudioHandle
    {
        public AudioSource AudioSource;

        public AudioHandle(AudioSource audioSource)
        {
            AudioSource = audioSource;
        }
        
        public bool IsPlaying => AudioSource != null && AudioSource.isPlaying;
        public float PanStereo
        {
            get => AudioSource != null ? AudioSource.panStereo : 0;
            set
            {
                if (AudioSource != null)
                {
                    AudioSource.panStereo = value;
                }
            }
        }
        public float Volume { get { return AudioSource == null ? 0 : AudioSystem.Instance().GetVolume(this); } set { if ( AudioSource != null ) AudioSystem.Instance().SetVolume(this,value); } }

        public void Stop(float overTime = 0.0f) 
        { 		
            if ( AudioSystem.IsValid() && AudioSource.isPlaying && overTime > 0.0f )
            {
                AudioSystem.Stop(this, overTime);  
                return;
            }

            if ( AudioSource != null )
            {
                AudioSource.gameObject.SetActive(false);
                AudioSource = null;
            }

            // Call through to SystemAudio to ensure removed from list
            AudioSystem.Stop(this);  
        }
    }
}