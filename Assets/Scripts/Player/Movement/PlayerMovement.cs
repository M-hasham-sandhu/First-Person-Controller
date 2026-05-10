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
        }

        private void FixedUpdate()
        {
            MovePlayer();
            JumpPlayer();
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