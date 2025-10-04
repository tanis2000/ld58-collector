using System;
using UnityEngine;

namespace GameBase.Animations
{
    public class CopyCameraRotation : MonoBehaviour
    {
        private Camera cam;

        private void OnEnable()
        {
            cam = Camera.main;
        }

        private void LateUpdate()
        {
            transform.rotation = cam.transform.rotation;
        }
    }
}