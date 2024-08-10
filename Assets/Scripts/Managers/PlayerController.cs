using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

namespace FPSTemplate
{
    public class PlayerController : BaseManager
    {
        bool isGrounded = true;

        public float acceleration = 200f; // how fast you accelerate
        public float accSprintMultiplier = 2; // how much faster you go when "sprinting"
        public float lookSensitivity = 3f; // mouse look sensitivity
        public float dampingCoefficient = 10f; // how quickly you break to a halt after you stop your input

        Vector3 velocity; // current velocity

        /*
         * Player
         */
        Quaternion Rotation;
        Vector3 Translation = Vector3.zero;

        GameObject playerController;
        GameObject playerArms;
        GameObject playerCockpit;

        GameObject[] muzzleEffects;
        FireFlame muzzleLeft;
        FireFlame muzzleRight;

        float playerArmsOffset = 0f;
        float playerCockpitOffset = 0f;

        Camera playerCamera;

        GameObject leftWeapon;
        GameObject rightWeapon;

        float verticalSensivity = 30f;
        float horizontalSensivity = 30f;

        private float rotationX = 0f;
        private float rotationY = 0f;

        public bool Active = true;

        /*
         * Head Bubbing
         */
        float headBubbingAmplitude = 0.01f;
        float headBubbingFrequency = 2f;
        float headBubbingVelocityThreshold = 0.1f;

        /*
         * Firing
         */
        bool isFiring = false;

        float minigunAceleration = 1500f;

        float minigunDeceleration = 800f;
        float minigunMaxSpeed = 3000f;
        float minigunInertia = 5f;

        float minigunCurrentSpeed = 0f;
        float minigunTargetSpeed = 0f;

        float minigunFireRate = 0.25f;
        float minigunLastFire = 0f;

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

            playerArms = GameObject.Find("Arms");
            playerCockpit = GameObject.Find("Cockpit");

            muzzleEffects = new GameObject[5];
            for (int i=0; i < 5 ; i++)
            {
                muzzleEffects[i] = GameObject.Find("muzzleFlash" + i);
                if (muzzleEffects[i] != null)
                    Debug.Log("found " + muzzleEffects[i].name.ToString());
            }

            muzzleLeft = GameObject.Find("LeftMuzzle").GetComponent<FireFlame>();
            muzzleRight = GameObject.Find("RightMuzzle").GetComponent<FireFlame>(); 
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

            // Firing
            isFiring = Input.GetMouseButton(0);

            if (isFiring)
            {
                minigunTargetSpeed += minigunAceleration * Time.deltaTime;
                minigunTargetSpeed = Mathf.Clamp(minigunTargetSpeed, 0, minigunMaxSpeed);
            }
            else
            {
                minigunTargetSpeed -= minigunDeceleration * Time.deltaTime;
                if (minigunTargetSpeed < 0)
                    minigunTargetSpeed = 0;
            }

            minigunCurrentSpeed = Mathf.Lerp(minigunCurrentSpeed, minigunTargetSpeed, minigunInertia * Time.deltaTime);

            if (minigunCurrentSpeed > 200 & isFiring)
            {
                minigunLastFire += Time.deltaTime;

                if (minigunLastFire >= minigunFireRate)
                {
                    minigunLastFire = 0;
                    Fire();
                }
                

            }
            leftWeapon.transform.Rotate(Vector3.up * -minigunCurrentSpeed * Time.deltaTime);
            rightWeapon.transform.Rotate(Vector3.up * minigunCurrentSpeed * Time.deltaTime);
        }

        float GetHeadBubbingOffset(float velocity, float h)
        {
            float offset = 0;

            if (velocity >= headBubbingVelocityThreshold)
                offset = h + Mathf.Sin(Time.time * headBubbingFrequency) * headBubbingAmplitude;

            return offset;
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
            muzzleLeft.Fire();
            muzzleRight.Fire();
        }

        public override void Dispose()
        {
        }

    
    }

}
