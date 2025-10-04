using System.Collections;
using UnityEngine;

namespace GameBase.Animations
{
    public class TweenAction
    {
        public enum Type
        {
            Position,
            LocalPosition,
            Rotation,
            Scale,
            Color
        };

        public Transform TheObject;
        public SpriteRenderer Sprite;
        public Vector3 StartPos, TargetPos;
        public Quaternion StartRot, TargetRot;
        public Color StartColor, TargetColor;
        public float TweenPos, TweenDuration, TweenDelay;
        public int CustomEasing;
        public Type TweenActionType;
        public System.Func<float, float> EaseFunction;

        private bool hasBeenInit;

        public Vector3 Lerp(Vector3 start, Vector3 end, float time)
        {
            if (CustomEasing >= 0)
            {
                return Vector3.LerpUnclamped(start, end, time);
            }
            else
            {
                return Vector3.Lerp(start, end, time);
            }
        }

        public Quaternion Lerp(Quaternion start, Quaternion end, float time)
        {
            if (CustomEasing >= 0)
            {
                return Quaternion.LerpUnclamped(start, end, time);
            }
            else
            {
                return Quaternion.Lerp(start, end, time);
            }
        }

        public Color Lerp(Color start, Color end, float time)
        {
            if (CustomEasing >= 0)
            {
                return Color.LerpUnclamped(start, end, time);
            }
            else
            {
                return Color.Lerp(start, end, time);
            }
        }

        public float DoEase()
        {
            if (CustomEasing >= 0)
            {
                return Tweener.Instance.CustomEasings[CustomEasing].Evaluate(TweenPos);
            }
            else
            {
                return EaseFunction(TweenPos);
            }
        }

        public IEnumerator SetStartPos()
        {
            yield return new WaitForSeconds(TweenDelay);
            hasBeenInit = true;
            StartPos = TheObject.transform.position;
        }

        public IEnumerator SetStartLocalPos()
        {
            yield return new WaitForSeconds(TweenDelay);
            hasBeenInit = true;
            StartPos = TheObject.transform.localPosition;
        }

        public IEnumerator SetStartRot()
        {
            yield return new WaitForSeconds(TweenDelay);
            hasBeenInit = true;
            StartRot = TheObject.transform.rotation;
        }

        public IEnumerator SetStartScale()
        {
            yield return new WaitForSeconds(TweenDelay);
            hasBeenInit = true;
            StartPos = TheObject && TheObject.transform ? TheObject.transform.localScale : Vector3.zero;
        }

        public IEnumerator SetStartColor()
        {
            yield return new WaitForSeconds(TweenDelay);
            hasBeenInit = true;
            StartColor = Sprite.color;
        }

        public bool Process()
        {
            if (!TheObject)
            {
                return true;
            }

            if (!hasBeenInit)
                return false;

            if (TweenDelay > 0f)
            {
                TweenDelay -= Time.deltaTime;
            }
            else
            {
                TweenPos += Time.deltaTime / TweenDuration;

                if (TweenActionType == Type.Position)
                {
                    TheObject.position = Lerp(StartPos, TargetPos, DoEase());
                }

                if (TweenActionType == Type.LocalPosition)
                {
                    TheObject.localPosition = Lerp(StartPos, TargetPos, DoEase());
                }

                if (TweenActionType == Type.Rotation)
                {
                    TheObject.rotation = Lerp(StartRot, TargetRot, DoEase());
                }

                if (TweenActionType == Type.Scale)
                {
                    TheObject.localScale = Lerp(StartPos, TargetPos, DoEase());
                }

                if (TweenActionType == Type.Color)
                {
                    Sprite.color = Lerp(StartColor, TargetColor, DoEase());
                }
            }

            return (TweenPos >= 1f);
        }
    }
}