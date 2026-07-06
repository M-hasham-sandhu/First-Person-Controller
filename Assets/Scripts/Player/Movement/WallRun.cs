using UnityEngine;

namespace Player.Movement
{
    public class WallRun : MonoBehaviour
    {
        public enum Side { None, Left, Right }

        [Header("Run Settings")]
        [SerializeField] private float maxWallRunTime = 1.5f;
        [SerializeField] private float wallRunCooldown = 0.5f;
        [SerializeField] private float minForwardInput = 0.1f;
        [SerializeField] private float minMoveSpeed = 3f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs;
        [SerializeField] private string blockReason;

        private float _runTimeRemaining;
        private float _cooldownTimer;

        public bool IsWallRunning { get; private set; }
        public Side CurrentSide { get; private set; }
        public float RunTimeRemaining => _runTimeRemaining;
        public string BlockReason => blockReason;

        private void Awake()
        {
            _runTimeRemaining = maxWallRunTime;
        }

        /// <summary>
        /// Advances the wall-run state. Call once per frame with fresh detection/input data.
        /// </summary>
        public void Tick(bool wallLeft, bool wallRight, bool isGrounded, float verticalInput, float horizontalSpeed)
        {
            UpdateTimers();

            if (isGrounded && !IsWallRunning)
                ResetRunTime();

            if (isGrounded)
            {
                SetBlockReason("Grounded");
                StopRun();
                return;
            }

            if (verticalInput < minForwardInput)
            {
                SetBlockReason("Not moving forward");
                StopRun();
                return;
            }

            if (horizontalSpeed < minMoveSpeed)
            {
                SetBlockReason("Too slow to wall run");
                StopRun();
                return;
            }

            Side side = ResolveSide(wallLeft, wallRight);

            if (side == Side.None)
            {
                SetBlockReason("No wall to run on");
                StopRun();
                return;
            }

            if (!CanSpendRunTime())
            {
                SetBlockReason("Wall run time depleted");
                StopRun();
                return;
            }

            StartRun(side);
        }

        private Side ResolveSide(bool wallLeft, bool wallRight)
        {
            // Stick to the current side if it's still valid, so the player doesn't
            // flip-flop when both sides briefly register a hit.
            if (IsWallRunning)
            {
                if (CurrentSide == Side.Left && wallLeft) return Side.Left;
                if (CurrentSide == Side.Right && wallRight) return Side.Right;
            }

            if (wallRight) return Side.Right;
            if (wallLeft) return Side.Left;
            return Side.None;
        }

        private void UpdateTimers()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            if (!IsWallRunning)
                return;

            _runTimeRemaining -= Time.deltaTime;

            if (_runTimeRemaining <= 0f)
            {
                _runTimeRemaining = 0f;
                _cooldownTimer = wallRunCooldown;
                StopRun();
            }
        }

        private bool CanSpendRunTime()
        {
            return _runTimeRemaining > 0f && _cooldownTimer <= 0f;
        }

        private void StartRun(Side side)
        {
            if (!IsWallRunning && showDebugLogs)
                Debug.Log($"Wall run started on {side} side");

            CurrentSide = side;
            SetBlockReason($"Running on {side} wall");
            IsWallRunning = true;
        }

        /// <summary>
        /// Immediately ends the wall run (e.g. player jumps off, or climbing takes over).
        /// </summary>
        public void StopRun()
        {
            if (!IsWallRunning)
            {
                CurrentSide = Side.None;
                return;
            }

            IsWallRunning = false;
            CurrentSide = Side.None;
        }

        private void ResetRunTime()
        {
            _runTimeRemaining = maxWallRunTime;
            _cooldownTimer = 0f;
        }

        private void SetBlockReason(string reason)
        {
            if (blockReason == reason)
                return;

            blockReason = reason;

            if (showDebugLogs)
                Debug.Log($"Wall run state: {blockReason}");
        }
    }
}