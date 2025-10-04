﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Animations
{
    public class Tweener : MonoBehaviour
    {
        public AnimationCurve[] CustomEasings;
        public bool UpdateOnLate;

        private List<TweenAction> actions;

        private static Tweener instance = null;
        public static Tweener Instance => instance;

        private void Awake()
        {
            instance = this;

            actions = new List<TweenAction>();
        }

        private void Update()
        {
            if (UpdateOnLate) return;
            Process();
        }

        private void LateUpdate()
        {
            if (!UpdateOnLate) return;
            Process();
        }

        private void Process()
        {
            for (var i = actions.Count - 1; i >= 0; i--)
            {
                if (actions[i].Process())
                {
                    actions.RemoveAt(i);
                }
            }
        }

        private TweenAction AddTween(Transform obj, Vector3 target, TweenAction.Type type, float duration, float delay,
            System.Func<float, float> ease, int easeIndex = -1, bool removeOld = true)
        {
            // remove old ones of same object
            if (removeOld)
            {
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    if (actions[i].TheObject == obj && actions[i].TweenActionType == type)
                    {
                        actions.RemoveAt(i);
                    }
                }
            }

            var act = new TweenAction
            {
                TweenActionType = type,
                TheObject = obj,
                TargetPos = target,
                TweenPos = 0f,
                TweenDuration = duration,
                TweenDelay = delay,
                CustomEasing = easeIndex
            };
            actions.Add(act);

            act.EaseFunction = ease;

            return act;
        }

        public static void MoveToBounceOut(Transform obj, Vector3 target, float duration, float delay = 0f)
        {
            Instance.MoveTo(obj, target, duration, delay, TweenEasings.BounceEaseOut);
        }

        public static void MoveToQuad(Transform obj, Vector3 target, float duration, float delay = 0f)
        {
            Instance.MoveTo(obj, target, duration, delay, TweenEasings.QuadraticEaseInOut);
        }

        public static void MoveTo(Transform obj, Vector3 target, float duration, System.Func<float, float> ease = null,
            float delay = 0f)
        {
            Instance.MoveTo(obj, target, duration, delay, ease);
        }

        public void MoveTo(Transform obj, Vector3 target, float duration, float delay,
            System.Func<float, float> ease = null, int easeIndex = -1, bool removeOld = true)
        {
            ease ??= TweenEasings.LinearInterpolation;
            var act = AddTween(obj, target, TweenAction.Type.Position, duration, delay, ease, easeIndex, removeOld);
            act.StartPos = act.TheObject.position;
            StartCoroutine(act.SetStartPos());
        }

        public void MoveLocalTo(Transform obj, Vector3 target, float duration, float delay,
            System.Func<float, float> ease = null, int easeIndex = -1, bool removeOld = true)
        {
            ease ??= TweenEasings.LinearInterpolation;
            var act = AddTween(obj, target, TweenAction.Type.LocalPosition, duration, delay, ease, easeIndex,
                removeOld);
            act.StartPos = act.TheObject.localPosition;
            StartCoroutine(act.SetStartLocalPos());
        }

        public static void MoveLocalToBounceOut(Transform obj, Vector3 target, float duration, float delay = 0f)
        {
            Instance.MoveLocalTo(obj, target, duration, delay, TweenEasings.BounceEaseOut);
        }

        public static void MoveLocalToQuad(Transform obj, Vector3 target, float duration, float delay = 0f)
        {
            Instance.MoveLocalTo(obj, target, duration, delay, TweenEasings.QuadraticEaseInOut);
        }

        public static void MoveLocalTo(Transform obj, Vector3 target, float duration,
            System.Func<float, float> ease = null, float delay = 0f)
        {
            Instance.MoveLocalTo(obj, target, duration, delay, ease);
        }

        public static void RotateToBounceOut(Transform obj, Quaternion rotation, float duration, float delay = 0f)
        {
            Instance.RotateTo(obj, rotation, duration, delay, TweenEasings.BounceEaseOut);
        }

        public static void RotateToQuad(Transform obj, Quaternion rotation, float duration, float delay = 0f)
        {
            Instance.RotateTo(obj, rotation, duration, delay, TweenEasings.QuadraticEaseInOut);
        }

        public void RotateTo(Transform obj, Quaternion rotation, float duration, float delay,
            System.Func<float, float> ease = null, int easeIndex = -1, bool removeOld = true)
        {
            ease ??= TweenEasings.LinearInterpolation;
            var act = AddTween(obj, Vector3.zero, TweenAction.Type.Rotation, duration, delay, ease, easeIndex,
                removeOld);
            act.StartRot = act.TheObject.rotation;
            act.TargetRot = rotation;
            StartCoroutine(act.SetStartRot());
        }

        public static void ScaleToBounceOut(Transform obj, Vector3 target, float duration, float delay = 0f)
        {
            Instance.ScaleTo(obj, target, duration, delay, TweenEasings.BounceEaseOut);
        }

        public static void ScaleToQuad(Transform obj, Vector3 target, float duration, float delay = 0f)
        {
            Instance.ScaleTo(obj, target, duration, delay, TweenEasings.QuadraticEaseInOut);
        }

        public static void ScaleTo(Transform obj, Vector3 target, float duration, System.Func<float, float> ease = null,
            float delay = 0f)
        {
            Instance.ScaleTo(obj, target, duration, delay, ease);
        }

        public void ScaleTo(Transform obj, Vector3 target, float duration, float delay,
            System.Func<float, float> ease = null, int easeIndex = -1, bool removeOld = true)
        {
            ease ??= TweenEasings.LinearInterpolation;
            var act = AddTween(obj, target, TweenAction.Type.Scale, duration, delay, ease, easeIndex, removeOld);
            StartCoroutine(act.SetStartScale());
        }

        public static void ColorToBounceOut(SpriteRenderer obj, Color target, float duration, float delay = 0f)
        {
            Instance.ColorTo(obj, target, duration, delay, TweenEasings.BounceEaseOut);
        }

        public static void ColorToQuad(SpriteRenderer obj, Color target, float duration, float delay = 0f)
        {
            Instance.ColorTo(obj, target, duration, delay, TweenEasings.QuadraticEaseInOut);
        }

        public void ColorTo(SpriteRenderer obj, Color color, float duration, float delay,
            System.Func<float, float> ease = null, int easeIndex = -1, bool removeOld = true)
        {
            ease ??= TweenEasings.LinearInterpolation;
            var act = AddTween(obj.transform, Vector3.zero, TweenAction.Type.Color, duration, delay, ease, easeIndex,
                removeOld);
            act.Sprite = obj;
            act.StartColor = act.Sprite.color;
            act.TargetColor = color;
            StartCoroutine(act.SetStartColor());
        }
    }
}