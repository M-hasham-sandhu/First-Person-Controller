using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(GroundDetector))]
    public class WallClimb : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;

        [Header("Wall Check")]
        [SerializeField] private LayerMask wallMask = ~0;
        [SerializeField] private float wallCheckDistance = 0.8f;
        [SerializeField] private float minWallAngle = 75f;

        [Header("Climb Settings")]
        [SerializeField] private float climbSpeed = 4f;
        [SerializeField] private float maxClimbTime = 1.25f;
        [SerializeField] private float climbCooldown = 0.35f;
        [SerializeField] private float minForwardInput = 0.1f;
        [SerializeField] private float wallStickSpeed = 1.5f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs;
        [SerializeField] private string blockReason;

        private Rigidbody _rb;
        private PlayerInput _input;
        private GroundDetector _groundDetector;

        private RaycastHit _wallHit;
        private float _climbTimeRemaining;
        private float _cooldownTimer;

        public bool IsClimbing { get; private set; }
        public bool WallInFront { get; private set; }
        public bool WantsToClimb { get; private set; }
        public Vector3 WallNormal => _wallHit.normal;
        public float ClimbTimeRemaining => _climbTimeRemaining;
        public string BlockReason => blockReason;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _input = GetComponent<PlayerInput>();
            _groundDetector = GetComponent<GroundDetector>();

            ResolveOrientation();

            _climbTimeRemaining = maxClimbTime;
        }

        private void Update()
        {
            UpdateTimers();
            CheckWall();

            if (_groundDetector.IsGrounded && !IsClimbing)
                ResetClimbTime();

            WantsToClimb = _input.ClimbHeld && _input.VerticalInput > minForwardInput;

            if (!WantsToClimb)
            {
                SetBlockReason("Waiting for climb input");
                StopClimb();
                return;
            }

            if (!WallInFront)
            {
                SetBlockReason("No climbable wall in front");
                StopClimb();
                return;
            }

            if (!CanSpendClimbTime())
            {
                SetBlockReason("Climb time depleted");
                StopClimb();
                return;
            }

            StartClimb();
        }

        private void FixedUpdate()
        {
            if (!IsClimbing)
                return;

            _rb.useGravity = false;

            Vector3 stickVelocity = -_wallHit.normal * wallStickSpeed;
            _rb.linearVelocity = new Vector3(stickVelocity.x, climbSpeed, stickVelocity.z);
        }

        private void UpdateTimers()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            if (!IsClimbing)
                return;

            _climbTimeRemaining -= Time.deltaTime;

            if (_climbTimeRemaining <= 0f)
            {
                _climbTimeRemaining = 0f;
                _cooldownTimer = climbCooldown;
                StopClimb();
            }
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

        private bool CanSpendClimbTime()
        {
            return _climbTimeRemaining > 0f && _cooldownTimer <= 0f;
        }

        private void StartClimb()
        {
            if (!IsClimbing && showDebugLogs)
                Debug.Log("Wall climb started");

            SetBlockReason("Climbing");
            IsClimbing = true;
        }

        private void StopClimb()
        {
            if (!IsClimbing)
                return;

            IsClimbing = false;
            _rb.useGravity = true;
        }

        private void ResetClimbTime()
        {
            _climbTimeRemaining = maxClimbTime;
            _cooldownTimer = 0f;
        }

        private void SetBlockReason(string reason)
        {
            if (blockReason == reason)
                return;

            blockReason = reason;

            if (showDebugLogs)
                Debug.Log($"Wall climb state: {blockReason}");
        }

        private void OnDrawGizmosSelected()
        {
            Transform rayOrientation = orientation != null ? orientation : transform;

            Gizmos.color = WallInFront ? Color.cyan : Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + rayOrientation.forward * wallCheckDistance);
        }
    }
}
