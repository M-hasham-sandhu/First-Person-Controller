using UnityEngine;

namespace Player.Movement
{
    [DefaultExecutionOrder(-50)]
    public class WallDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;

        [Header("Wall Check")]
        [SerializeField] private LayerMask wallMask = ~0;
        [SerializeField] private float wallCheckDistance = 0.8f;
        [SerializeField] private float minWallAngle = 75f;

        private RaycastHit _wallHit;

        public bool WallInFront { get; private set; }
        public RaycastHit WallHit => _wallHit;
        public Vector3 WallNormal => _wallHit.normal;

        private void Awake()
        {
            ResolveOrientation();
        }

        private void Update()
        {
            CheckWall();
        }

        private void CheckWall()
        {
            ResolveOrientation();

            WallInFront = Physics.Raycast(
                transform.position,
                orientation.forward,
                out _wallHit,
                wallCheckDistance,
                wallMask
            ) && IsValidClimbWall(_wallHit.normal);
        }

        private bool IsValidClimbWall(Vector3 wallNormal)
        {
            return Vector3.Angle(Vector3.up, wallNormal) >= minWallAngle;
        }

        private void ResolveOrientation()
        {
            if (orientation != null)
                return;

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            orientation = playerMovement != null && playerMovement.Orientation != null
                ? playerMovement.Orientation
                : transform;
        }

        private void OnDrawGizmosSelected()
        {
            Transform rayOrientation = orientation != null ? orientation : transform;

            Gizmos.color = WallInFront ? Color.cyan : Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + rayOrientation.forward * wallCheckDistance);
        }
    }
}
