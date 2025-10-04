using System;
using System.Collections;
using UnityEngine;

namespace GameBase.Transitions
{
    public class TransitionSystem : MonoBehaviour
    {
        public SpriteRenderer FadeSpritePrefab;
        public Color FadeColor = Color.black;

        private static TransitionSystem instance;
        private SpriteRenderer fadeSprite;

        public static TransitionSystem Instance()
        {
            return instance;
        }

        private void OnEnable()
        {
            instance = this;
        }

        public IEnumerator FadeIn(float time, string source)
        {
            var alpha = 1.0f;
            if (fadeSprite == null)
            {
                fadeSprite = Instantiate(FadeSpritePrefab, Vector3.zero, Quaternion.identity);
            }
            fadeSprite.color = new Color(FadeColor.r,FadeColor.g,FadeColor.b,alpha);
            var cooldown = time;
            while (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                if (cooldown < 0)
                {
                    cooldown = 0;
                }

                alpha = cooldown / time;
                fadeSprite.color = new Color(FadeColor.r,FadeColor.g,FadeColor.b,alpha);
                yield return new WaitForEndOfFrame();
            }
            Destroy(fadeSprite.gameObject);
            yield return null;
        }

        public IEnumerator FadeOut(float time, string source)
        {
            var alpha = 0.0f;
            if (fadeSprite == null) {
                fadeSprite = Instantiate(FadeSpritePrefab, Vector3.zero, Quaternion.identity);
            }
            fadeSprite.color = new Color(FadeColor.r,FadeColor.g,FadeColor.b,alpha);
            var cooldown = 0.0f;
            while (cooldown < time)
            {
                cooldown += Time.deltaTime;
                if (cooldown > time)
                {
                    cooldown = time;
                }

                alpha = cooldown / time;
                fadeSprite.color = new Color(FadeColor.r,FadeColor.g,FadeColor.b,alpha);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        public IEnumerator FadeSkip()
        {
            yield return FadeOut(0.05f, "");
        }
    }
}