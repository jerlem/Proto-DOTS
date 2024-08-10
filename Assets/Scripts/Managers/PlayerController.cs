using System.Globalization;
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

        GameObject leftWeapon;
        GameObject rightWeapon;

        float verticalSensivity = 30f;
        float horizontalSensivity = 30f;

        private float rotationX = 0f;
        private float rotationY = 0f;

        public bool Active = true;

        bool isFiring = false;

        public override void Init()
        {
            GameManager.EventManager.OnKeyPress += HandleKeyPressed;
            GameManager.EventManager.OnKeyReleased += HandleKeyReleased;

            GameManager.EventManager.OnMouseClick += HandleMouseClick;
            GameManager.EventManager.OnMouseReleased += HandleMouseRelease;
            GameManager.EventManager.OnMouseScroll += HandleMouseScroll;

            playerController = GameObject.Find("Player");
            playerCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

            leftWeapon = GameObject.Find("Left_Minigun_M-134_Guns");
            rightWeapon = GameObject.Find("Right_Minigun_M-134_Guns");
        }

        /// <summary>
        /// Handling Mouse click event
        /// </summary>
        /// <param name="button"></param>
        private void HandleMouseClick(int button)
        {
        }

        /// <summary>
        /// Handling Mouse released event
        /// </summary>
        /// <param name="button"></param>
        private void HandleMouseRelease(int button)
        {
        }

        /// <summary>
        /// Handle Mouse scrolling
        /// </summary>
        /// <param name="y"></param>
        private void HandleMouseScroll(float y)
        {

        }

        /// <summary>
        /// Handle Key pressed
        /// </summary>
        /// <param name="keyCode"></param>
        private void HandleKeyPressed(KeyCode keyCode) => HandleKey(keyCode, true);

        /// <summary>
        /// Handle Key released
        /// </summary>
        /// <param name="keyCode"></param>
        private void HandleKeyReleased(KeyCode keyCode) => HandleKey(keyCode, false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="pressed"></param>
        private void HandleKey(KeyCode keyCode, bool pressed)
        {
           
        }

        //void SetMovementKeys()
        //{
        //    // Detect system culture to set movement keys
        //    string culture = CultureInfo.CurrentCulture.Name;
        //    if (culture == "fr-FR") // Example for French AZERTY layout
        //    {
        //        _moveForward = KeyCode.Z;
        //        _moveBackward = KeyCode.S;
        //        _moveLeft = KeyCode.Q;
        //        _moveRight = KeyCode.D;
        //    }
        //    else // Default to QWERTY layout
        //    {
        //        _moveForward = KeyCode.W;
        //        _moveBackward = KeyCode.S;
        //        _moveLeft = KeyCode.A;
        //        _moveRight = KeyCode.D;
        //    }
        //}

        public void FrameUpdate()
        {
            if (!Active)
                return;

            UpdateLookDirection();

            // Movement
            velocity += GetAccelerationVector() * Time.deltaTime;
            velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
            playerController.transform.position += velocity * Time.deltaTime;

            isFiring = Input.GetMouseButtonDown(0);

            if (isFiring) Fire();
        }

        Vector3 GetAccelerationVector()
        {
            Vector3 moveInput = default;

            void AddMovement(KeyCode key, Vector3 dir)
            {
                if (Input.GetKey(key))
                    moveInput += dir;
            }

            AddMovement(KeyCode.W, Vector3.forward);
            AddMovement(KeyCode.A, Vector3.left);
            AddMovement(KeyCode.S, Vector3.back);
            AddMovement(KeyCode.D, Vector3.right);

            //AddMovement(KeyCode.Space, Vector3.up);
            //AddMovement(KeyCode.LeftControl, Vector3.down);

            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.z).normalized;

            direction = playerController.transform.TransformDirection(direction);

            if (Input.GetKey(KeyCode.LeftShift))
                return direction * (acceleration * accSprintMultiplier);

            return direction * acceleration;
        }


        private void UpdateLookDirection()
        {
            float mouseX = Input.GetAxis("Mouse X") * horizontalSensivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * verticalSensivity * Time.deltaTime;

            rotationY += mouseX;
            rotationX += mouseY; // cam

            rotationX = Mathf.Clamp(rotationX, -50f, 50f);

            playerCamera.transform.localRotation = Quaternion.Euler(-rotationX, 0f, 0f);
            playerController.transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);

        }


        public void Move()
        {

        }

        public void Jump()
        {

        }

        public void Fire()
        {
            Debug.Log("Fire");
            leftWeapon.transform.Rotate(Vector3.up * 100 * Time.deltaTime);
            rightWeapon.transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }

        public override void Dispose()
        {
        }

    
    }

}
