using UnityEngine;

namespace GameBase.Animations
{
    public class Follower : MonoBehaviour
    {
        public Transform Target;

        private void Update()
        {
            transform.position = Target.position;
        }
    }
}