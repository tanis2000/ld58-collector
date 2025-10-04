using GameBase.Audio;
using TMPro;
using UnityEngine;

namespace GameBase.Animations
{
    public class Appearer : MonoBehaviour
    {
        public AudioCue Sounds;
        public float AppearAfter = -1f;
        public float HideDelay;
        public bool Silent;
        public GameObject Visuals;
        public TMP_Text Text;
        
        private Vector3 size;

        private void Awake()
        {
            var t = transform;
            size = t.localScale;
            t.localScale = Vector3.zero;
            if(Visuals) Visuals.SetActive(false);

            if (AppearAfter >= 0)
                Invoke(nameof(Show), AppearAfter);
        }

        public void ShowAfter()
        {
            Invoke(nameof(Show), AppearAfter);
        }

        public void ShowAfter(float delay)
        {
            Invoke(nameof(Show), delay);
        }

        public void Show()
        {
            CancelInvoke(nameof(Hide));
            CancelInvoke(nameof(MakeInactive));
            DoSound();

            if(Visuals) Visuals.SetActive(true);
            Tweener.Instance.ScaleTo(transform, size, 0.3f, 0f, TweenEasings.BounceEaseOut);
        }

        public void Hide()
        {
            CancelInvoke(nameof(Show));
            DoSound();

            Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        
            if(Visuals) Invoke(nameof(MakeInactive), 0.2f);
        }

        private void MakeInactive()
        {
            Visuals.SetActive(false);
        }

        private void DoSound()
        {
            if (Silent) return;

            if (Sounds != null)
            {
                AudioSystem.Instance().Play(Sounds, transform);
            }
        }

        public void HideWithDelay()
        {
            Invoke(nameof(Hide), HideDelay);
        }
        
        public void HideWithDelay(float delay)
        {
            Invoke(nameof(Hide), delay);
        }

        public void ShowWithText(string t, float delay)
        {
            if (Text)
                Text.text = t;

            Invoke(nameof(Show), delay);
        }
    }
}
