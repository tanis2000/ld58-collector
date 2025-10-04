using GameBase.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameBase.SceneChanger
{
    public class SceneChanger : MonoBehaviour
    {
        public Blinders Blinders;
        public Transform Spinner;

        public string SceneToLoadAtStart;

        public Canvas Canvas;
        public GameObject StartCam;
        private AsyncOperation operation;

        private string sceneToLoad;

        public static SceneChanger Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(Instance.gameObject);
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(SceneToLoadAtStart))
                ChangeScene(SceneToLoadAtStart, true);
        }

        private void Update()
        {
            if (operation != null && operation.isDone)
            {
                operation = null;
                Invoke("After", 0.1f);
            }
        }

        public void AttachCamera()
        {
            Canvas.worldCamera = Camera.main;
        }

        private void After()
        {
            Blinders.Open();
            Tweener.Instance.ScaleTo(Spinner, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseIn);
        }

        public void ChangeScene(string sceneName, bool silent = false, bool closeBlinders = true)
        {
            if (!silent)
            {
                //AudioManager.Instance.DoButtonSound();
            }

            if (StartCam)
                StartCam.SetActive(true);

            if (closeBlinders)
            {
                Blinders.Close();
                Tweener.Instance.ScaleTo(Spinner, Vector3.one, 0.2f, 0f, TweenEasings.BounceEaseOut);
            }

            sceneToLoad = sceneName;
            CancelInvoke(nameof(DoChangeScene));
            Invoke(nameof(DoChangeScene), Blinders.GetDuration());
        }

        private void DoChangeScene()
        {
            operation = SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }
}