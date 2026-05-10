using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float airMultiplier = 0.5f;
        
        [Header("Ground Check")]
        [SerializeField] private float playerHeight = 2f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;

        [Header("Drag Settings")]
        [SerializeField] private float groundDrag = 0.5f;
        [SerializeField] private float airDrag = 0.75f;
        
        [Header("Speed Limits")]
        [SerializeField] private float maxSpeed = 6f;
        
        private Rigidbody _rb;
        private CapsuleCollider _col;
        private float _horizontalInput;
        private float _verticalInput;
        private bool _isGrounded;
        private bool _jumpPressed;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponentInChildren<CapsuleCollider>();
        }

        private void Update()
        {
            ReadInput();
            CheckGround();
            ApplyDrag();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            JumpPlayer();
            LimitSpeed();
        }

        private void ReadInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKeyDown(KeyCode.Space))
                _jumpPressed = true;
        }
        
        private void MovePlayer()
        {
            Vector3 moveDirection =
                orientation.forward * _verticalInput +
                orientation.right * _horizontalInput;

            _rb.AddForce(
                moveDirection.normalized * (moveSpeed ),
                ForceMode.Force
            );
        }
        
        private void CheckGround()
        {
            _isGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                playerHeight * 0.5f + groundCheckDistance,
                groundMask
            );
        }
        
        private void JumpPlayer()
        {
            if (_jumpPressed && _isGrounded)
            {
                Vector3 velocity = _rb.linearVelocity;
                velocity.y = 0;
                _rb.linearVelocity = velocity;

                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            _jumpPressed = false;
        }
        
        private void ApplyDrag()
        {
            _rb.linearDamping = _isGrounded ? groundDrag : airDrag;
        }
        
        private void LimitSpeed()
        {
            Vector3 velocity = _rb.linearVelocity;

            Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

            float maxSpeed = this.maxSpeed;

            if (flatVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                Vector3 limited = flatVelocity.normalized * maxSpeed;

                _rb.linearVelocity = new Vector3(
                    limited.x,
                    velocity.y,
                    limited.z
                );
            }
        }
        
        private void OnDrawGizmos()
        {
            if (_col == null) return;

            Gizmos.color = _isGrounded ? Color.green : Color.red;

            Vector3 origin = transform.position;
            Vector3 direction = Vector3.down;
            float distance = playerHeight * 0.5f + groundCheckDistance;

            Gizmos.DrawLine(origin, origin + direction * distance);
            Gizmos.DrawWireSphere(origin + direction * distance, 0.1f);
        }
        
    }
}