using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameBase.Effects
{
    public class EffectsSystem : MonoBehaviour
    {
        public AutoEnd[] Effects;
        public TextPopup TextPopupPrefab;

        private Queue<AutoEnd>[] effectPool;
        private Queue<TextPopup> textPopupPool;

        public static EffectsSystem Instance { get; private set; }

        // ==================

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            effectPool = new Queue<AutoEnd>[Effects.Length];

            for (var i = 0; i < effectPool.Length; i++)
            {
                effectPool[i] = new Queue<AutoEnd>();
            }

            textPopupPool = new Queue<TextPopup>();
        }

        private GameObject DoAddEffect(int effect, Vector3 position, float angle = 0f)
        {
            var e = Get(effect);
            var t = e.transform;
            t.parent = transform;
            t.position = position;
            t.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            return e.gameObject;
        }

        public GameObject AddEffectToParent(int effect, Vector3 position, Transform parent)
        {
            var e = Get(effect);
            var t = e.transform;
            t.parent = parent;
            t.position = position;
            return e.gameObject;
        }

        private AutoEnd Get(int index)
        {
            if (!effectPool[index].Any())
                AddObjects(index, 1);

            var obj = effectPool[index].Dequeue();
            obj.gameObject.SetActive(true);
            obj.Start();
            obj.GetParticleSystem().Play();

            return obj;
        }

        private TextPopup GetTextPopup()
        {
            if (!textPopupPool.Any())
            {
                textPopupPool.Enqueue(Instantiate(TextPopupPrefab));
            }

            return textPopupPool.Dequeue();
        }

        private void AddObjects(int index, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = Instantiate(Effects[index]);
                obj.Pool = index;
                effectPool[index].Enqueue(obj);
            }
        }

        public void ReturnToPool(AutoEnd obj)
        {
            obj.gameObject.SetActive(false);
            effectPool[obj.Pool].Enqueue(obj);
        }

        public void ReturnToPool(TextPopup obj)
        {
            obj.gameObject.SetActive(false);
            textPopupPool.Enqueue(obj);
        }

        public static void AddEffects(IEnumerable<int> ids, Vector3 position)
        {
            foreach (var id in ids)
            {
                Instance.DoAddEffect(id, position);
            }
        }

        public static GameObject AddEffect(int id, Vector3 position, float angle = 0f)
        {
            var eff = Instance.DoAddEffect(id, position, angle);
            return eff.gameObject;
        }

        public static GameObject AddTextPopup(string content, Vector3 position)
        {
            var t = Instance.GetTextPopup();
            t.transform.SetParent(Instance.transform);
            t.transform.position = position;
            t.Play(content);
            var go = t.gameObject;
            go.SetActive(true);
            return go;
        }
    }
}