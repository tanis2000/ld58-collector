using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameBase.Effects
{
    public class TextPopup : MonoBehaviour
    {
        public List<TMP_Text> Texts;
        public float Duration = 5f;

        private Animator anim;
        private int defaultState;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void Play(string content)
        {
            Texts.ForEach(t => t.text = content);
            Invoke(nameof(Done), Duration);

            if (!anim) return;
            anim.Play(defaultState, -1, 0);
        }

        private void Done()
        {
            EffectsSystem.Instance.ReturnToPool(this);
        }
    }
}