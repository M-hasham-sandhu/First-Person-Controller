using UnityEngine;

namespace Camera
{
    public class PlayerCam : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform playerOrientation;
        
        [Header("Mouse Settings")]
        [SerializeField] private float mouseSensitivity = 100f;

        [Header("Clamp Settings")]
        [SerializeField] private float minLookAngle = -90f;
        [SerializeField] private float maxLookAngle = 90f;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleMouseLook();
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxisRaw("Mouse X")
                           * mouseSensitivity
                           * Time.deltaTime;

            float mouseY = Input.GetAxisRaw("Mouse Y")
                           * mouseSensitivity
                           * Time.deltaTime;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            
            _xRotation = Mathf.Clamp(
                _xRotation,
                minLookAngle,
                maxLookAngle
            );

            cameraHolder.localRotation = Quaternion.Euler(
                _xRotation,
                _yRotation,
                0f
            );
            
            playerOrientation.localRotation = Quaternion.Euler(
                0f,
                _yRotation,
                0f);
        }
    }
}
