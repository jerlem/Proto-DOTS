using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

namespace FPSTemplate
{
    public class PlayerController : BaseManager
    {
        bool isGrounded = true;

        public float acceleration = 600f; // how fast you accelerate
        public float accSprintMultiplier = 4; // how much faster you go when "sprinting"
        public float lookSensitivity = 3f; // mouse look sensitivity
        public float dampingCoefficient = 10f; // how quickly you break to a halt after you stop your input

        Vector3 velocity; // current velocity

        Quaternion Rotation;
        Vector3 Translation = Vector3.zero;
        GameObject playerController;
        Camera playerCamera;

        float verticalSensivity = 30f;
        float horizontalSensivity = 30f;

        private float rotationX = 0f;
        private float rotationY = 0f;

        public bool Active = true;

        public void FrameUpdate()
        {
            if (!Active)
                return;

            UpdateLookDirection();

            //// Position
            velocity += GetAccelerationVector() * Time.deltaTime;

            //// Physics
            velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
            playerController.transform.position += velocity * Time.deltaTime;
        }

        Vector3 GetAccelerationVector()
        {
            Vector3 moveInput = default;

            void AddMovement(KeyCode key, Vector3 dir)
            {
                if (Input.GetKey(key))
                    moveInput += dir;
            }

            //AddMovement(KeyCode.W, Vector3.right);
            //AddMovement(KeyCode.A, Vector3.forward);
            //AddMovement(KeyCode.S, Vector3.left);
            //AddMovement(KeyCode.D, Vector3.back);

            AddMovement(KeyCode.W, Vector3.forward);
            AddMovement(KeyCode.A, Vector3.left);
            AddMovement(KeyCode.S, Vector3.back);
            AddMovement(KeyCode.D, Vector3.right);

            AddMovement(KeyCode.Space, Vector3.up);
            AddMovement(KeyCode.LeftControl, Vector3.down);

            // Supprimer les mouvements verticaux pour rester sur le plan horizontal
            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.z).normalized;

            direction = playerController.transform.TransformDirection(direction);

            if (Input.GetKey(KeyCode.LeftShift))
                return direction * (acceleration * accSprintMultiplier); // "sprinting"

            return direction * acceleration; // "walking"
        }

        public override void Init()
        {
            Debug.Log("Init player controller");

            playerController = GameObject.Find("Player");
            playerCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        }

        private void UpdateLookDirection()
        {
            float mouseX = Input.GetAxis("Mouse X") * horizontalSensivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * verticalSensivity * Time.deltaTime;

            rotationY += mouseX;
            rotationX += -mouseY; // cam

            rotationX = Mathf.Clamp(rotationX, -40f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            playerController.transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);

        }


        public void Move()
        {

        }

        public void Jump()
        {

        }

        public override void Dispose()
        {
        }

    
    }

}
