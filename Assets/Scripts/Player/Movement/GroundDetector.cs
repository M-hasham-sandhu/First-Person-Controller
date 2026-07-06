using UnityEngine;

namespace Player.Movement
{
    public class GroundDetector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float playerHeight = 2f;
        [SerializeField] private float checkDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float maxSlopeAngle = 45f;

        public bool IsGrounded { get; private set; }
        public bool OnSlope { get; private set; }
        public RaycastHit SlopeHit { get; private set; }

        private void Update()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            IsGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                playerHeight * 0.5f + checkDistance,
                groundMask
            );

            OnSlope = CheckOnSlope();
        }

        private bool CheckOnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + checkDistance, groundMask))
            {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                SlopeHit = hit;
                return angle < maxSlopeAngle && angle != 0;
            }
            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, SlopeHit.normal).normalized;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Vector3 origin = transform.position;
            Vector3 direction = Vector3.down;
            float distance = playerHeight * 0.5f + checkDistance;

            Gizmos.DrawLine(origin, origin + direction * distance);
            Gizmos.DrawWireSphere(origin + direction * distance, 0.1f);
        }
    }
}
