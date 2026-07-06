using UnityEngine;

namespace Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        private void Update()
        {
            if (target != null)
            {
                transform.position = target.transform.position;
            }
        }
    }
}