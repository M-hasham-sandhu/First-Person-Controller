using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementDirectionOrientation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;

        [Header("Settings")]
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private float minMoveSpeed = 0.1f;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            if (target == null)
                target = transform;
        }

        private void FixedUpdate()
        {
            Vector3 moveDirection = _rb.linearVelocity;
            moveDirection.y = 0f;

            if (moveDirection.sqrMagnitude < minMoveSpeed * minMoveSpeed)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            target.rotation = Quaternion.Slerp(
                target.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
        }
    }
}
