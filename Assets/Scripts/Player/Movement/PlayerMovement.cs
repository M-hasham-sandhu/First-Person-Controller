using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(GroundDetector))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 6f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float crouchSpeed = 3f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float airMultiplier = 0.5f;

        [Header("Crouch Settings")]
        [SerializeField] private float crouchYScale = 0.5f;
        private float _startYScale;
        
        [Header("Drag Settings")]
        [SerializeField] private float groundDrag = 5f;
        [SerializeField] private float airDrag = 0f;

        [Header("Wall Climb Movement")]
        [SerializeField] private float climbSpeed = 4f;
        [SerializeField] private float wallStickSpeed = 1.5f;

        [Header("Wall Run Movement")]
        [SerializeField] private float wallRunSpeed = 8f;
        [SerializeField] private float wallStickForce = 15f;
        [SerializeField] private float wallRunFallSpeed = 1.5f;
        [SerializeField] private float wallRunMaxFallSpeed = -1.5f;

        [Header("Wall Run Visuals")]
        [SerializeField] private float wallRunTiltAngle = 60f;
        [SerializeField] private float wallRunTiltSpeed = 8f;

        private Rigidbody _rb;
        private PlayerInput _input;
        private GroundDetector _groundDetector;
        private WallClimb _wallClimb;
        private WallDetector _wallDetector;
        private WallRun _wallRun;
        private WallRunDetector _wallRunDetector;
        private WallJump _wallJump;
        private Camera.PlayerCam _playerCam;

        private float _moveSpeed;

        public enum MovementState
        {
            Walking,
            Sprinting,
            Crouching,
            Climbing,
            WallRunning,
            Air
        }

        public MovementState state;

        public Transform Orientation => orientation;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _input = GetComponent<PlayerInput>();
            _groundDetector = GetComponent<GroundDetector>();
            _wallClimb = GetComponent<WallClimb>();
            _wallDetector = GetComponent<WallDetector>();
            _wallRun = GetComponent<WallRun>();
            _wallRunDetector = GetComponent<WallRunDetector>();
            _wallJump = GetComponent<WallJump>();
            _playerCam = FindObjectOfType<Camera.PlayerCam>();

            _startYScale = transform.localScale.y;
            
            // Set initial damping
            _rb.freezeRotation = true;
        }

        private void Update()
        {
            UpdateWallClimb();
            UpdateWallRun();
            StateHandler();
            ApplyDrag();
            UpdateWallRunTilt();
        }

        private void FixedUpdate()
        {
            if (IsWallClimbing())
            {
                ClimbPlayer();
                _wallJump?.Tick(_input.JumpPressed);
                _input.ResetJump();
                return;
            }

            if (IsWallRunning())
            {
                WallRunPlayer();
                _wallJump?.Tick(_input.JumpPressed);
                _input.ResetJump();
                return;
            }

            MovePlayer();
            JumpPlayer();
            LimitSpeed();
        }

        private void StateHandler()
        {
            if (IsWallClimbing())
            {
                state = MovementState.Climbing;
                _moveSpeed = 0f;
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
                return;
            }

            if (IsWallRunning())
            {
                state = MovementState.WallRunning;
                _moveSpeed = wallRunSpeed;
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
                return;
            }

            // Mode - Crouching
            if (_input.CrouchHeld)
            {
                state = MovementState.Crouching;
                _moveSpeed = crouchSpeed;
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
                
                // Mode - Sprinting
                if (_groundDetector.IsGrounded && _input.SprintHeld)
                {
                    state = MovementState.Sprinting;
                    _moveSpeed = sprintSpeed;
                }
                // Mode - Walking
                else if (_groundDetector.IsGrounded)
                {
                    state = MovementState.Walking;
                    _moveSpeed = walkSpeed;
                }
                // Mode - Air
                else
                {
                    state = MovementState.Air;
                }
            }
        }
        
        private void MovePlayer()
        {
            Vector3 moveDirection = orientation.forward * _input.VerticalInput + orientation.right * _input.HorizontalInput;

            // On Slope
            if (_groundDetector.OnSlope)
            {
                _rb.AddForce(_groundDetector.GetSlopeMoveDirection(moveDirection) * (_moveSpeed * 20f), ForceMode.Force);

                if (_rb.linearVelocity.y > 0)
                    _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            // Grounded
            else if (_groundDetector.IsGrounded)
            {
                _rb.AddForce(moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
            }
            // In Air
            else
            {
                _rb.AddForce(moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
            }

            // Turn off gravity while on slope to avoid sliding
            _rb.useGravity = !_groundDetector.OnSlope;
        }
        
        private void JumpPlayer()
        {
            if (_input.JumpPressed && _groundDetector.IsGrounded)
            {
                Vector3 velocity = _rb.linearVelocity;
                velocity.y = 0;
                _rb.linearVelocity = velocity;

                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            _input.ResetJump();
        }
        
        private void ApplyDrag()
        {
            if (IsWallClimbing() || IsWallRunning())
            {
                _rb.linearDamping = airDrag;
                return;
            }

            _rb.linearDamping = _groundDetector.IsGrounded ? groundDrag : airDrag;
        }

        private bool IsWallClimbing()
        {
            return _wallClimb != null && _wallClimb.IsClimbing;
        }

        private bool IsWallRunning()
        {
            return _wallRun != null && _wallRun.IsWallRunning;
        }

        private void UpdateWallClimb()
        {
            if (_wallClimb == null)
                return;

            _wallClimb.Tick(
                _input.ClimbHeld,
                _input.VerticalInput,
                _groundDetector.IsGrounded,
                _wallDetector != null && _wallDetector.WallInFront
            );
        }

        private void UpdateWallRun()
        {
            if (_wallRun == null || _wallRunDetector == null)
                return;

            // Wall climbing (facing the wall) takes priority over wall running (beside the wall)
            if (IsWallClimbing())
            {
                _wallRun.StopRun();
                return;
            }

            Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            _wallRun.Tick(
                _wallRunDetector.WallLeft,
                _wallRunDetector.WallRight,
                _groundDetector.IsGrounded,
                _input.VerticalInput,
                flatVelocity.magnitude
            );
        }

        private void ClimbPlayer()
        {
            _rb.useGravity = false;

            Vector3 wallNormal = _wallDetector != null && _wallDetector.WallInFront
                ? _wallDetector.WallNormal
                : -orientation.forward;

            Vector3 stickVelocity = -wallNormal * wallStickSpeed;
            _rb.linearVelocity = new Vector3(stickVelocity.x, climbSpeed, stickVelocity.z);
        }

        private void WallRunPlayer()
        {
            _rb.useGravity = false;

            Vector3 wallNormal = _wallRun.CurrentSide == WallRun.Side.Right
                ? _wallRunDetector.RightHit.normal
                : _wallRunDetector.LeftHit.normal;

            // Direction along the wall, facing the same way the player is looking
            Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up).normalized;
            if (Vector3.Dot(wallForward, orientation.forward) < 0f)
                wallForward = -wallForward;

            // Keep the player glued to the wall while they slide along it
            _rb.AddForce(-wallNormal * wallStickForce, ForceMode.Force);

            // Gradual, capped fall so the run naturally peters out rather than holding altitude forever
            float verticalVelocity = Mathf.Max(
                _rb.linearVelocity.y - wallRunFallSpeed * Time.fixedDeltaTime,
                wallRunMaxFallSpeed
            );

            _rb.linearVelocity = new Vector3(
                wallForward.x * wallRunSpeed,
                verticalVelocity,
                wallForward.z * wallRunSpeed
            );
        }

        
        private void LimitSpeed()
        {
            // Limit speed on slope
            if (_groundDetector.OnSlope)
            {
                if (_rb.linearVelocity.magnitude > _moveSpeed)
                    _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
            }
            // Limit speed on ground or in air
            else
            {
                Vector3 velocity = _rb.linearVelocity;
                Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

                if (flatVelocity.magnitude > _moveSpeed)
                {
                    Vector3 limited = flatVelocity.normalized * _moveSpeed;
                    _rb.linearVelocity = new Vector3(limited.x, velocity.y, limited.z);
                }
            }
        }

        private void UpdateWallRunTilt()
        {
            if (_playerCam == null)
                return;

            float targetRoll = 0f;
            if (IsWallRunning())
            {
                targetRoll = _wallRun.CurrentSide == WallRun.Side.Right
                    ? wallRunTiltAngle
                    : -wallRunTiltAngle;
            }

            _playerCam.SetTargetRoll(targetRoll, wallRunTiltSpeed);
        }
    }
}