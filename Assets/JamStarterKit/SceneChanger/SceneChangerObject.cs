using GameBase.Utils;
using UnityEngine;

namespace GameBase.SceneChanger
{


    public class SceneChangerObject : MonoBehaviour
    {
        public void ChangeScene(string scene)
        {
            SceneChanger.Instance.ChangeScene(scene);
        }

        public void Quit()
        {
            SceneChanger.Instance.Blinders.Close();
            this.StartCoroutine(Application.Quit, 1f);
        }

        public void ToMainOrName()
        {
            ChangeScene(PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name");
        }
    }
}