using UnityEngine;
using System.Globalization;

public class FreeFlyingCamera : MonoBehaviour
{
    public float movementSpeed = 10f;  // Speed of movement
    public float lookSpeed = 3f;       // Speed of mouse look
    public float shiftMultiplier = 2f; // Speed multiplier when holding Shift
    public KeyCode toggleKey = KeyCode.Tab; // Key to toggle camera on/off

    private Vector3 _moveDirection;
    private float _rotationX;
    private float _rotationY;
    private bool _isCameraActive = true;

    // Movement keys
    private KeyCode _moveForward;
    private KeyCode _moveBackward;
    private KeyCode _moveLeft;
    private KeyCode _moveRight;

    void Start()
    {
        SetMovementKeys();
        ToggleCamera(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            _isCameraActive = !_isCameraActive;
            ToggleCamera(_isCameraActive);
        }

        if (_isCameraActive)
        {
            // Handle movement input
            HandleMovementInput();

            // Handle mouse look
            HandleMouseLook();

            // Move the camera
            MoveCamera();
        }
    }

    void SetMovementKeys()
    {
        // Detect system culture to set movement keys
        string culture = CultureInfo.CurrentCulture.Name;

        if (culture == "fr-FR") // Example for French AZERTY layout
        {
            _moveForward = KeyCode.Z;
            _moveBackward = KeyCode.S;
            _moveLeft = KeyCode.Q;
            _moveRight = KeyCode.D;
        }
        else // Default to QWERTY layout
        {
            _moveForward = KeyCode.W;
            _moveBackward = KeyCode.S;
            _moveLeft = KeyCode.A;
            _moveRight = KeyCode.D;
        }
    }

    void HandleMovementInput()
    {
        float moveSpeed = movementSpeed;

        // Increase speed when holding Shift
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed *= shiftMultiplier;
        }

        // Movement input based on dynamic keys
        _moveDirection = Vector3.zero;
        if (Input.GetKey(_moveForward))
            _moveDirection += Vector3.forward;
        if (Input.GetKey(_moveBackward))
            _moveDirection += Vector3.back;
        if (Input.GetKey(_moveLeft))
            _moveDirection += Vector3.left;
        if (Input.GetKey(_moveRight))
            _moveDirection += Vector3.right;

        _moveDirection = transform.TransformDirection(_moveDirection) * moveSpeed;
    }

    void HandleMouseLook()
    {
        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;

        // Update rotation angles
        _rotationX += mouseX;
        _rotationY += mouseY;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f); // Limit vertical rotation

        // Apply rotation to the camera
        transform.localRotation = Quaternion.Euler(_rotationY, _rotationX, 0);
    }

    void MoveCamera()
    {
        // Apply movement
        transform.position += _moveDirection * Time.deltaTime;
    }

    void ToggleCamera(bool isActive)
    {
        if (isActive)
        {
            // Lock the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
