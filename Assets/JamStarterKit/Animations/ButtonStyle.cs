using System;
using System.Collections.Generic;
using System.Linq;
using GameBase.Audio;
using GameBase.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameBase.Animations
{
    public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        public bool AnimateColors;
        public List<Image> BgImages;
        public List<Image> FgImages;
        public List<TMP_Text> Texts;
        public List<Color> BgColors = new() { Color.black };
        public List<Color> FgColors = new() { Color.white };

        public bool AnimateScale;
        public float ScaleAmount;

        public bool AnimateRotation;
        public float RotationAmount;

        public AudioCue ClickSound;
        public AudioCue HoverSound;

        private Vector3 originalScale;
        private Color originalBackColor;
        private Color originalFrontColor;

        private void Start()
        {
            originalScale = transform.localScale;
            SaveOriginalColors();
        }
        
        private void SaveOriginalColors()
        {
            if (!AnimateColors) return;
            
            BgImages.ForEach(i =>
            {
                originalBackColor = i.color;
            });

            FgImages.ForEach(i =>
            {
                originalFrontColor = i.color;
            });
            
            if (!Texts.Any()) return;
            originalFrontColor = Texts.First().color;
        }

        public void Select()
        {
            ApplyScaling(ScaleAmount, TweenEasings.BounceEaseOut);
            ApplyRotation(Random.Range(-RotationAmount, RotationAmount), TweenEasings.BounceEaseOut);
            ApplyColors(BgColors.Random(), FgColors.Random());
            PlaySound(HoverSound);
        }

        public void Deselect()
        {
            PlaySound(HoverSound);
            Reset();
        }

        public void OnPointerEnter(PointerEventData eventData)
        { 
            Select();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Deselect();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlaySound(ClickSound);
        }

        private void PlaySound(AudioCue cue)
        {
            if (cue == null)
            {
                return;
            }
            AudioSystem.Instance().Play(cue);
        }
        
        private void ApplyScaling(float amount, Func<float, float> easing)
        {
            if (!AnimateScale)
            {
                return;
            }
            Tweener.Instance.ScaleTo(transform, originalScale * (1f + amount), 0.2f, 0f, easing);
        }
        
        private void ApplyRotation(float amount, Func<float, float> easing)
        {
            if (!AnimateRotation)
            {
                return;
            }
            Tweener.Instance.RotateTo(transform, Quaternion.Euler(0, 0, amount), 0.2f, 0f, easing);
        }
        
        private void ApplyColors(Color back, Color front)
        {
            if (!AnimateColors) return;
            
            BgImages.ForEach(i => i.color = back);
            FgImages.ForEach(i => i.color = front);
            
            if (!Texts.Any()) return;
            
            Texts.ForEach(t => t.color = front);
        }
        
        public void Reset()
        {
            ApplyScaling(0, TweenEasings.BounceEaseOut);
            ApplyRotation(0, TweenEasings.BounceEaseOut);
            ApplyColors(originalBackColor, originalFrontColor);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Select();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Deselect();
        }
    }
}