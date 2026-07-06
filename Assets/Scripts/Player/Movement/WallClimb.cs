using UnityEngine;

namespace Player.Movement
{
    public class WallClimb : MonoBehaviour
    {
        [Header("Climb Settings")]
        [SerializeField] private float maxClimbTime = 1.25f;
        [SerializeField] private float climbCooldown = 0.35f;
        [SerializeField] private float minForwardInput = 0.1f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs;
        [SerializeField] private string blockReason;

        private float _climbTimeRemaining;
        private float _cooldownTimer;

        public bool IsClimbing { get; private set; }
        public bool WantsToClimb { get; private set; }
        public float ClimbTimeRemaining => _climbTimeRemaining;
        public string BlockReason => blockReason;

        private void Awake()
        {
            _climbTimeRemaining = maxClimbTime;
        }

        public void Tick(bool climbHeld, float forwardInput, bool isGrounded, bool wallInFront)
        {
            UpdateTimers();

            if (isGrounded && !IsClimbing)
                ResetClimbTime();

            WantsToClimb = climbHeld && forwardInput > minForwardInput;

            if (!WantsToClimb)
            {
                SetBlockReason("Waiting for climb input");
                StopClimb();
                return;
            }

            if (!wallInFront)
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
        }

        public void ForceStopClimb()
        {
            if (!IsClimbing)
                return;

            IsClimbing = false;
            _cooldownTimer = climbCooldown;
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
    }
}
