using System.Collections.Generic;
using GameBase.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace GameBase.Audio
{
    public class AudioSystem : MonoBehaviour
    {
        public int AudioSourcePoolSize = 16;
        public AudioSource AudioSourcePrefab;
        public List<AudioTypeVolume> VolumeByType = new List<AudioTypeVolume>();

        [Header("Default falloff values (ratios of screen width)")]
        public float FalloffMinVol = 0.2f;

        public float FalloffPanMax = 0.75f;

        public float FalloffStart = 1.2f;
        public float FalloffEnd = 1.2f;

        public float FalloffPanStart = 0.5f;
        public float FalloffPanEnd = 2.0f;

        [Header("List of audio cues that can be played by name")]
        [Tooltip(
            "If set, any cues in the Audio folder will be automatically added (you don't have to click the button in the cue)")]
        public bool AutoAddCues = true;

        [Tooltip("Audio cues that can be played by name")]
        public List<AudioCue> AudioCues = new List<AudioCue>();

        private string AudioSourceNamePrefix = "Audio: ";
        private static AudioSystem instance;
        private List<AudioSource> audioSources = new List<AudioSource>();
        private AudioListener audioListener;
        private List<AudioHandle> defaultAudioHandles = new List<AudioHandle>();
        private Camera gameCamera;
        private List<ClipInfo> activeAudio = new List<ClipInfo>();
        private XRandom rnd = new XRandom(42);

        private void OnEnable()
        {
            instance = this;

            audioListener = FindObjectOfType<AudioListener>();
            if (audioListener == null)
            {
                Debug.LogWarning("Unable to find audio listener in scene");
            }

            gameCamera = Camera.main;

            for (var i = 0; i < AudioSourcePoolSize; i++)
            {
                var audioSource = Instantiate(AudioSourcePrefab, Vector3.zero, Quaternion.identity, transform);
                var go = audioSource.gameObject;
                go.name = AudioSourceNamePrefix;
                go.SetActive(false);
                audioSources.Add(audioSource);
            }

            AudioCues.RemoveAll(item => item == null);
        }

        private void Update()
        {
            transform.position = audioListener.transform.position;
            UpdateActiveAudio();
        }

        public static AudioSystem Instance()
        {
            if (instance == null)
            {
                instance = (AudioSystem)FindObjectOfType(typeof(AudioSystem));
                if (instance == null)
                {
                    GameObject container = new GameObject();
                    container.name = typeof(AudioSystem).ToString();
                    instance = (AudioSystem)container.AddComponent(typeof(AudioSystem));
                    
                    //if ( Application.isPlaying == false )
                    //    container.hideFlags = HideFlags.HideAndDontSave;
                }
            }

            return instance;
        }

        public static bool IsValid()
        {
            return instance != null;
        }


        /// Sets the volume (from 0.0 to 1.0) for audio of a particular type (eg: Sound effects, Music, Dialog) 
        public static void SetVolume(AudioType type, float volume)
        {
            AudioSystem self = AudioSystem.Instance();
            if (self == null)
            {
                Debug.LogWarning("Failed to set AudioSystem volume. It hasn't been initialised");
                return;
            }

            float oldVolume = 1.0f;
            bool exists = false;

            if (volume <= 0) // If volume goes to zero we lose the scale information. So make it -1 instead... mathyhack!
                volume = -1;

            for (int i = 0; i < self.VolumeByType.Count; ++i)
            {
                if (self.VolumeByType[i].AudioType == type)
                {
                    oldVolume = self.VolumeByType[i].Volume;
                    self.VolumeByType[i].Volume = volume;
                    exists = true;
                    break;
                }
            }

            if (exists == false)
            {
                // Doesn't exist yet, so add
                self.VolumeByType.Add(new AudioTypeVolume() { AudioType = type, Volume = volume });
            }

            float volChange = volume / oldVolume;
            // Update active audio volumes
            for (int i = 0; i < self.activeAudio.Count; ++i)
            {
                ClipInfo info = self.activeAudio[i];
                if (info != null && info.AudioType == type)
                {
                    if (info.Handle.AudioSource != null)
                        info.Handle.AudioSource.volume *= volChange;
                    info.DefaultVolume *= volChange;
                }
            }
        }

        /// Retrieves the volume for audio of a particular type (eg: Sound effects, Music, Dialog) 
        public static float GetVolume(AudioType type)
        {
            AudioSystem self = AudioSystem.Instance();
            for (int i = 0; i < self.VolumeByType.Count; ++i)
            {
                if (self.VolumeByType[i].AudioType == type)
                {
                    return Mathf.Clamp01(self.VolumeByType[i].Volume);
                }
            }

            return 1.0f;
        }

        /// Gets the base volume of a particular sound effect (usually use the AudioHandle for this)
        public float GetVolume(AudioHandle source)
        {
            ClipInfo clipInfo = instance.activeAudio.Find(item => item.Handle == source);
            if (clipInfo != null)
                return clipInfo.DefaultVolume;
            return 0;
        }

        /// Sets the base volume of a particular sound effect (usually use the AudioHandle for this)
        public void SetVolume(AudioHandle source, float volume)
        {
            ClipInfo clipInfo = instance.activeAudio.Find(item => item.Handle == source);
            if (clipInfo != null &&
                clipInfo.StopAfterFade ==
                false) // NB: once stop after fade is called, don't allow volume changes through this function			
            {
                clipInfo.DefaultVolume = volume;
                clipInfo.TargetVolume = volume;
            }
        }

        /// Play a cue by name. This is the main way to play sounds. If emmitter is set, the sound will falloff/pan as it goes off camera. 
        /**
         * Eg: `Audio.Play("DoorKnock");`
         * Eg: `Audio.Play("Gunshot", C.Gunman.Instance);`
         */
        public AudioHandle Play(string cueName, Transform emitter = null)
        {
            AudioCue cue = AudioCues.Find(item =>
                string.Equals(cueName, item.name, System.StringComparison.OrdinalIgnoreCase));
            if (cue == null && string.IsNullOrEmpty(cueName) == false)
            {
                Debug.LogWarning("Sound cue not found: " + cueName);
            }

            return Play(cue, emitter);
        }

        /// Play the specified cue with extended options. If emmitter is set, the sound will falloff/pan as it goes off camera. Can also set volume and pitch overrides, and override the start time of the cue
        public AudioHandle Play(AudioCue cue, Transform emitter = null, float volumeMul = 1, float pitchMul = 1,
            float startTime = 0)
        {
            return Play(cue, ref defaultAudioHandles, emitter, volumeMul, pitchMul, startTime);
        }

        /// Advanced Play function with extended options, and returning a list of all handles started (for when multiple are started). If emmitter is set, the sound will falloff/pan as it goes off camera. Can also set volume and pitch overrides, and override the start time of the cue
        public AudioHandle Play(AudioCue cue, ref List<AudioHandle> handles, Transform emitter = null,
            float volumeMul = 1, float pitchMul = 1, float startTime = 0)
        {
            if (cue == null)
            {
                return new AudioHandle(null);
            }

            var cueClip = cue.GetClip();

            if (cueClip == null)
            {
                return new AudioHandle(null);
            }

            if (rnd.Float() > cue.Chance)
            {
                return new AudioHandle(null);
            }

            var audioClip = cueClip.AudioClip;

            if (audioClip == null)
            {
                return new AudioHandle(null);
            }

            var volume = cueClip.Volume.GetRandom() * volumeMul;
            volume *= cue.Volume.GetRandom();

            AudioMixerGroup mixerGroup = null;
            foreach (var audioTypeVolume in VolumeByType)
            {
                if (audioTypeVolume.AudioType == cue.AudioType)
                {
                    volume *= audioTypeVolume.Volume;
                    mixerGroup = audioTypeVolume.MixerGroup;
                }
            }

            var pitch = cue.Pitch.GetRandom() * cueClip.Pitch.GetRandom() * pitchMul;
            var pan = cue.Pan.GetRandom();

            var source = SpawnAudioSource($"{AudioSourceNamePrefix}{audioClip.name}", transform.position);

            {
                var sourceFilter = source.gameObject.GetComponent<AudioReverbFilter>();
                if (sourceFilter == null)
                {
                    AddSourceFilters(source.gameObject);
                    sourceFilter = source.gameObject.GetComponent<AudioReverbFilter>();
                }

                sourceFilter.enabled = cue.ReverbPreset != AudioReverbPreset.Off;
                if (sourceFilter.enabled)
                {
                    sourceFilter.reverbPreset = cue.ReverbPreset;
                }
            }

            {
                var sourceFilter = source.gameObject.GetComponent<AudioEchoFilter>();
                sourceFilter.enabled = cue.EchoFilter != null;
                if (sourceFilter.enabled)
                {
                    sourceFilter.delay = cue.EchoFilter.delay;
                    sourceFilter.decayRatio = cue.EchoFilter.decayRatio;
                    sourceFilter.dryMix = cue.EchoFilter.dryMix;
                    sourceFilter.wetMix = cue.EchoFilter.wetMix;
                }
            }

            {
                var sourceFilter = source.gameObject.GetComponent<AudioDistortionFilter>();
                sourceFilter.enabled = cue.DistortionLevel > 0;
                if (sourceFilter.enabled)
                {
                    sourceFilter.distortionLevel = cue.DistortionLevel;
                }
            }

            {
                var sourceFilter = source.gameObject.GetComponent<AudioHighPassFilter>();
                sourceFilter.enabled = cue.HighPassFilter != null;
                if (sourceFilter.enabled)
                {
                    sourceFilter.cutoffFrequency = cue.HighPassFilter.cutoffFrequency;
                    sourceFilter.highpassResonanceQ = cue.HighPassFilter.highpassResonanceQ;
                }
            }

            {
                var sourceFilter = source.gameObject.GetComponent<AudioLowPassFilter>();
                sourceFilter.enabled = cue.LowPassFilter != null;
                if (sourceFilter.enabled)
                {
                    sourceFilter.cutoffFrequency = cue.LowPassFilter.cutoffFrequency;
                    sourceFilter.lowpassResonanceQ = cue.LowPassFilter.lowpassResonanceQ;
                }
            }

            {
                var sourceFilter = source.gameObject.GetComponent<AudioChorusFilter>();
                sourceFilter.enabled = cue.ChorusFilter != null;
                if (sourceFilter.enabled)
                {
                    sourceFilter.delay = cue.ChorusFilter.delay;
                    sourceFilter.depth = cue.ChorusFilter.depth;
                    sourceFilter.dryMix = cue.ChorusFilter.dryMix;
                    sourceFilter.wetMix1 = cue.ChorusFilter.wetMix1;
                    sourceFilter.wetMix2 = cue.ChorusFilter.wetMix2;
                    sourceFilter.wetMix3 = cue.ChorusFilter.wetMix3;
                    sourceFilter.rate = cue.ChorusFilter.rate;
                }
            }

            if (cue.MixerGroup != null)
            {
                source.outputAudioMixerGroup = cue.MixerGroup;
            }
            else
            {
                source.outputAudioMixerGroup = mixerGroup;
            }

            source.transform.parent = transform;
            source.transform.localPosition = Vector3.zero;

            SetSource(ref source, audioClip, volume, pitch, 128, emitter);
            source.loop = cue.Loop;
            source.panStereo = pan;

            if (startTime <= 0 && cueClip.StartTime > 0)
            {
                startTime = cueClip.StartTime;
            }

            if (startTime > 0)
            {
                source.time = Mathf.Min(startTime, (source.clip.length - 0.01f) * 0.9f);
            }
            else
            {
                source.time = 0;
            }

            source.Play();
            if (!source.isPlaying && source.time > 0)
            {
                Debug.LogWarning("Failed to play sound from specific time. Retrying from beginning");
                source.time = 0;
                source.Play();
            }

            var handle = new AudioHandle(source);
            if (handles != defaultAudioHandles)
            {
                if (handles == null)
                {
                    handles = new List<AudioHandle>();
                }

                handles.Add(handle);
            }

            if (cueClip.EndTime > 0)
            {
                var stopTime = cueClip.EndTime;
                if (cueClip.StartTime > 0)
                {
                    stopTime -= cueClip.StartTime;
                }

                stopTime /= pitch;
                handle.AudioSource.SetScheduledEndTime(AudioSettings.dspTime + stopTime);
            }

            AddActiveAudio(new ClipInfo
            {
                Handle = handle,
                Cue = cue,
                AudioType = cue.AudioType,
                DefaultVolume = volume,
                TargetVolume = volume,
                DefaultPitch = pitch,
                Emitter = emitter == null ? transform : emitter,
            });

            return handle;
        }

        /// Stops the specified sound by it's handle
        public static void Stop(AudioHandle handle, float overTime = 0)
        {
            if (instance && handle != null && handle.IsPlaying)
            {
                // This calls back to SystemAudio once "isPlaying" is set false, so the active audio can be removed. Hacky, but means Stop can be called either from here or the handle.
                if (overTime <= 0)
                    handle.Stop();
                else
                    instance.StartFade(handle, 0, overTime, true);
            }
        }

        AudioSource SpawnAudioSource(string name, Vector2 position)
        {
            AudioSource source = null;
            for (int i = 0; i < audioSources.Count; ++i)
            {
                source = audioSources[i];
                if (source == null)
                {
                    // source has been deleted, add new one in place
                    var sourceLoc = Instantiate(AudioSourcePrefab, Vector3.zero, Quaternion.identity, transform);
                    var go = sourceLoc.gameObject;
                    go.name = AudioSourceNamePrefix;
                    go.SetActive(false);
                    audioSources[i] = sourceLoc;
                    break;
                }
                else if (source.gameObject.activeSelf == false)
                {
                    break;
                }

                source = null;
            }

            if (source == null)
            {
                // no pooled source found, add new source
                var sourceLoc = Instantiate(AudioSourcePrefab, Vector3.zero, Quaternion.identity, transform);
                var go = sourceLoc.gameObject;
                go.name = AudioSourceNamePrefix;
                go.SetActive(false);
                audioSources.Add(sourceLoc);
                source = sourceLoc.GetComponent<AudioSource>();
            }

            if (source != null)
            {
                if (source.isPlaying)
                    Debug.Log("Reusing audio Source that's playing: " + source.clip.name);
                var go = source.gameObject;
                go.SetActive(true);
                go.name = name;
                go.transform.position = position;
            }
            else
            {
                Debug.Log("Failed to spawn audio source");
            }

            return source;
        }

        void AddSourceFilters(GameObject source)
        {
            {
                AudioLowPassFilter filter = source.AddComponent<AudioLowPassFilter>();
                filter.enabled = false;
            }
            {
                AudioHighPassFilter filter = source.AddComponent<AudioHighPassFilter>();
                filter.enabled = false;
            }
            {
                AudioEchoFilter filter = source.AddComponent<AudioEchoFilter>();
                filter.enabled = false;
            }
            {
                AudioChorusFilter filter = source.AddComponent<AudioChorusFilter>();
                filter.enabled = false;
            }
            {
                AudioDistortionFilter filter = source.AddComponent<AudioDistortionFilter>();
                filter.enabled = false;
            }
            {
                AudioReverbFilter filter = source.AddComponent<AudioReverbFilter>();
                filter.enabled = false;
            }
        }

        void SetSource(ref AudioSource source, AudioClip clip, float volume, float pitch, int priority,
            Transform emitter)
        {
            source.spatialize = false;
            source.priority = priority;
            source.pitch = pitch;
            source.clip = clip;
            source.playOnAwake = false;

            if (emitter)
            {
                source.volume = volume * GetFalloff(emitter.position);
                source.panStereo = GetPanPos(emitter.position);
            }
            else
            {
                source.volume = volume;
                source.panStereo = 0;
            }
        }

        float GetFalloff(Vector2 soundPos)
        {
            if (gameCamera == null)
                return 1.0f;
            float xStart =
                gameCamera.orthographicSize * gameCamera.aspect *
                FalloffStart; // Falloff starts half a screen past the edge
            float xFalloff = (xStart * FalloffEnd);
            return Mathf.Lerp(1, FalloffMinVol,
                Utils.Utils.EaseCubic((Mathf.Abs(soundPos.x - gameCamera.transform.position.x) - xStart) / xFalloff));
        }

        float GetPanPos(Vector2 soundPos)
        {
            if (gameCamera == null)
                return 0.0f;
            float xStart =
                gameCamera.orthographicSize * gameCamera.aspect *
                FalloffPanStart; // Falloff starts half a screen past the edge
            float xFalloff = (xStart * FalloffPanEnd);
            float dist = soundPos.x - gameCamera.transform.position.x;
            xFalloff = Mathf.Lerp(0, FalloffPanMax, Utils.Utils.EaseCubic((Mathf.Abs(dist) - xStart) / xFalloff));
            xFalloff *= Mathf.Sign(dist);
            return xFalloff;
        }

        // Adds clip to active audio for updating
        void AddActiveAudio(ClipInfo info)
        {
            activeAudio.Add(info);

            // Update activeAudio for this clip immediately in case it starts playing before the update loop
            UpdateActiveAudioClip(info);
        }

        // Updates all active audio
        void UpdateActiveAudio()
        {
            if (gameCamera == null)
                gameCamera = Camera.main;


            List<ClipInfo> toRemove = new List<ClipInfo>();
            foreach (ClipInfo audioClip in activeAudio)
            {
                if (audioClip.Handle == null ||
                    (audioClip.Handle.IsPlaying == false && audioClip.Paused == false))
                {
                    toRemove.Add(audioClip);
                }
                else
                {
                    UpdateActiveAudioClip(audioClip, 1);
                }
            }

            // Cleanup
            foreach (ClipInfo audioClip in toRemove)
            {
                activeAudio.Remove(audioClip);
                if (audioClip.Handle != null)
                    audioClip.Handle.Stop();
            }
        }

        // updates single active audio clip
        void UpdateActiveAudioClip(ClipInfo audioClip, float duckingVolume = 1.0f)
        {
            float volumeMod = 1.0f;
            float panMod = 0;

            if (audioClip.Emitter != null && audioClip.Emitter != transform)
            {
                volumeMod *= GetFalloff(audioClip.Emitter.position);
                panMod += GetPanPos(audioClip.Emitter.position);
                audioClip.Handle.PanStereo = panMod;
            }

            // Update Fading
            if (Utils.Utils.ApproximatelyZero(audioClip.FadeDelta) == false &&
                Mathf.Approximately(audioClip.TargetVolume, audioClip.DefaultVolume) == false)
            {
                audioClip.DefaultVolume = Mathf.MoveTowards(audioClip.DefaultVolume, audioClip.TargetVolume,
                    audioClip.FadeDelta * Time.deltaTime);
                // If flagged to stop on fadeout, stop the source
                if (audioClip.StopAfterFade && Mathf.Approximately(audioClip.TargetVolume, audioClip.DefaultVolume) &&
                    audioClip.Handle.IsPlaying)
                    audioClip.Handle.Stop();
            }

            // Duck music
            if (duckingVolume < 1.0f && audioClip.AudioType == AudioType.Music &&
                duckingVolume < audioClip.DefaultVolume)
            {
                volumeMod = duckingVolume / audioClip.DefaultVolume;
            }

            audioClip.Handle.Volume = audioClip.DefaultVolume * volumeMod;
        }

        /// Starts fading a handle to the target volume, optionally stopping it once finished. In powerquest it's usually more convenient to use `Audio.GetCue("MySound").Fade(targetVolume, overTime);`
        public void StartFade(AudioHandle handle, float targetVolume, float time, bool stopOnFinish = false)
        {
            ClipInfo clipInfo = activeAudio.Find(clip => clip.Handle == handle);
            if (clipInfo == null)
                return;

            float oldTarget = clipInfo.TargetVolume;

            clipInfo.TargetVolume = targetVolume;
            clipInfo.StopAfterFade = stopOnFinish;

            if (time <= 0)
            {
                clipInfo.DefaultVolume = targetVolume;
                if (stopOnFinish)
                    handle.Stop();
                return;
            }

            // Calc fade over time delta
            float dist = Mathf.Abs(targetVolume - oldTarget);
            if (dist <= 0)
            {
                if (stopOnFinish)
                    handle.Stop();
                return;
            }

            clipInfo.FadeDelta = dist / time;
        }

        /// Editor function for adding an audio cue to the list of cues playable by name. Primarily used by editor. Returns true if added, false if it already existed
        public bool EditorAddCue(AudioCue cue)
        {
            if (AudioCues.Contains(cue) == false)
            {
                AudioCues.Add(cue);
                return true;
            }

            return false;
        }

        public XRandom GetXRandom()
        {
            return rnd;
        }
    }
}