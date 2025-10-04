using System;
using GameBase.Utils;
using UnityEngine;

namespace GameBase.Audio
{
    [Serializable]
    public class Clip
    {
        public AudioClip AudioClip;
        public float Weight = 1;
        public MinMaxRange Volume = new MinMaxRange(1);
        public MinMaxRange Pitch = new MinMaxRange(1);
        public float StartTime;
        public float EndTime;
    }
}