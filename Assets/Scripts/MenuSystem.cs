using System;
using GameBase.Audio;
using UnityEngine;

namespace App
{
    public class MenuSystem: MonoBehaviour
    {
        private void OnEnable()
        {
            AudioSystem.Instance().Play("MusicMenu");
        }
    }
}