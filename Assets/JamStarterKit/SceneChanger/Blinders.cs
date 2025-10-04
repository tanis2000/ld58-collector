using GameBase.Animations;
using UnityEngine;

namespace GameBase.SceneChanger
{
    public class Blinders : MonoBehaviour
    {
        public Transform Left, Right;
        public bool StartsOpen, OpenAtStart = true;

        private const float Duration = 0.3f;
        private bool isOpen;

        // Start is called before the first frame update
        private void Start()
        {
            isOpen = StartsOpen;

            if (StartsOpen) return;

            Left.transform.localScale = new Vector3(1f, 5f, 1f);
            Right.transform.localScale = new Vector3(1f, 5f, 1f);

            if (OpenAtStart)
                Invoke(nameof(Open), 0.5f);
        }

        public void Close()
        {
            if (!isOpen) return;

            Tweener.Instance.ScaleTo(Left, new Vector3(1f, 2f, 1f), Duration, 0f, TweenEasings.BounceEaseOut);
            Tweener.Instance.ScaleTo(Right, new Vector3(1f, 2f, 1f), Duration, 0f, TweenEasings.BounceEaseOut);

            isOpen = false;
        }

        public void Open()
        {
            Tweener.Instance.ScaleTo(Left, new Vector3(0f, 2f, 1f), Duration, 0f, TweenEasings.BounceEaseOut);
            Tweener.Instance.ScaleTo(Right, new Vector3(0f, 2f, 1f), Duration, 0f, TweenEasings.BounceEaseOut);

            isOpen = true;
        }

        public float GetDuration()
        {
            return Duration;
        }
    }
}