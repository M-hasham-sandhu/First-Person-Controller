using UnityEngine;

namespace Player.Movement
{
    public class PlayerInput : MonoBehaviour
    {
        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool CrouchHeld { get; private set; }
        public bool ClimbHeld { get; private set; }

        private void Update()
        {
            HorizontalInput = Input.GetAxisRaw("Horizontal");
            VerticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
                JumpPressed = true;
            
            SprintHeld = Input.GetKey(KeyCode.LeftShift);
            CrouchHeld = Input.GetKey(KeyCode.LeftControl);
            ClimbHeld = Input.GetKey(KeyCode.E);
        }

        public void ResetJump()
        {
            JumpPressed = false;
        }
    }
}
