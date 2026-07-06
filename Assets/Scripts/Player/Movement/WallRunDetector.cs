using UnityEngine;

namespace Player.Movement
{
    [DefaultExecutionOrder(-50)]
    public class WallRunDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;

        [Header("Wall Check")]
        [SerializeField] private LayerMask wallMask = ~0;
        [SerializeField] private float wallCheckDistance = 0.7f;
        [SerializeField] private float minWallAngle = 75f;

        private RaycastHit _rightHit;
        private RaycastHit _leftHit;

        public bool WallRight { get; private set; }
        public bool WallLeft { get; private set; }
        public RaycastHit RightHit => _rightHit;
        public RaycastHit LeftHit => _leftHit;

        private void Awake()
        {
            ResolveOrientation();
        }

        private void Update()
        {
            CheckWalls();
        }

        private void CheckWalls()
        {
            ResolveOrientation();

            WallRight = Physics.Raycast(
                transform.position,
                orientation.right,
                out _rightHit,
                wallCheckDistance,
                wallMask
            ) && IsValidWallRunWall(_rightHit.normal);

            WallLeft = Physics.Raycast(
                transform.position,
                -orientation.right,
                out _leftHit,
                wallCheckDistance,
                wallMask
            ) && IsValidWallRunWall(_leftHit.normal);
        }

        private bool IsValidWallRunWall(Vector3 wallNormal)
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

            Gizmos.color = WallRight ? Color.cyan : Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + rayOrientation.right * wallCheckDistance);

            Gizmos.color = WallLeft ? Color.cyan : Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position - rayOrientation.right * wallCheckDistance);
        }
    }
}