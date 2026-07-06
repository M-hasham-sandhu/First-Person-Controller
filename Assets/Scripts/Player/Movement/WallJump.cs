using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class WallJump : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float wallJumpForce = 10f;
        [SerializeField] private float minWallAngle = 75f;

        private Rigidbody _rb;
        private PlayerInput _input;
        private WallClimb _wallClimb;
        private WallRun _wallRun;
        private WallDetector _wallDetector;
        private WallRunDetector _wallRunDetector;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _input = GetComponent<PlayerInput>();
            _wallClimb = GetComponent<WallClimb>();
            _wallRun = GetComponent<WallRun>();
            _wallDetector = GetComponent<WallDetector>();
            _wallRunDetector = GetComponent<WallRunDetector>();
        }

        public void Tick(bool jumpPressed)
        {
            if (!jumpPressed)
                return;

            Vector3 wallNormal;
            if (TryGetWallNormal(out wallNormal))
            {
                PerformWallJump(wallNormal);
            }
        }

        private bool TryGetWallNormal(out Vector3 wallNormal)
        {
            wallNormal = Vector3.zero;

            if (_wallClimb != null && _wallClimb.IsClimbing && _wallDetector != null && _wallDetector.WallInFront)
            {
                wallNormal = _wallDetector.WallNormal;
                return true;
            }

            if (_wallRun != null && _wallRun.IsWallRunning && _wallRunDetector != null)
            {
                if (_wallRun.CurrentSide == WallRun.Side.Right && _wallRunDetector.RightHit.collider != null)
                {
                    wallNormal = _wallRunDetector.RightHit.normal;
                    return true;
                }

                if (_wallRun.CurrentSide == WallRun.Side.Left && _wallRunDetector.LeftHit.collider != null)
                {
                    wallNormal = _wallRunDetector.LeftHit.normal;
                    return true;
                }
            }

            return false;
        }

        private void PerformWallJump(Vector3 wallNormal)
        {
            if (Vector3.Angle(Vector3.up, wallNormal) < minWallAngle)
                return;

            Vector3 velocity = _rb.linearVelocity;
            velocity.y = 0f;
            _rb.linearVelocity = velocity;

            Vector3 jumpImpulse = wallNormal.normalized * wallJumpForce;
            _rb.AddForce(jumpImpulse, ForceMode.Impulse);

            _wallClimb?.ForceStopClimb();
            _wallRun?.StopRun();
            _input.ResetJump();
        }
    }
}
