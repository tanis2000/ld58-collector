using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

namespace GameBase.Dev
{
    public class DevReload : MonoBehaviour
    {
        private InputAction reloadAction;

        private void Start()
        {
            reloadAction = InputSystem.actions.FindAction("Reload");
        }

        private void Update()
        {
            if (!Application.isEditor) return;

            reloadAction.ReadValue<float>();
            if (reloadAction.IsPressed())
            {
                SceneChanger.SceneChanger.Instance.ChangeScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}