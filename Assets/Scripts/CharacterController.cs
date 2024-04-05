
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody = default;
    [SerializeField] private float groundMovementSpeed = default;
    [SerializeField] private float airMovementSpeed = default;
    [SerializeField] private float mouseSensitivity = default;
    [SerializeField] private float jumpVelocity = default;
    [SerializeField] private Camera playerCamera = default;
    [SerializeField] private float verticalDrag = default;
    [SerializeField] private float horizontalDrag = default;

    private const KeyCode moveLeftKey = KeyCode.A;
    private const KeyCode moveRightKey = KeyCode.D;
    private const KeyCode moveForwardKey = KeyCode.W;
    private const KeyCode moveBackwardKey = KeyCode.S;
    private const KeyCode jumpKey = KeyCode.Space;
             
    private const KeyCode unlockMouse = KeyCode.Escape;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float xDifference = Input.GetAxis("Mouse X") * mouseSensitivity;
        float yDifference = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0, xDifference, 0);
        Vector3 cameraAngles = playerCamera.transform.localRotation.eulerAngles;
        cameraAngles = GetRealEulerAngle(cameraAngles);
        cameraAngles.x -= yDifference;
        cameraAngles.x = Mathf.Clamp(cameraAngles.x, -110, 110);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraAngles);
    }

    private Vector3 GetRealEulerAngle(Vector3 angle)
    {
        if (angle.x > 180)
            angle.x -= 360;

        if (angle.y != 180)
            return angle;

        if (angle.z != 180)
            return angle;

        if (angle.x > 0)
            angle.x = 180 - angle.x;
        else
            angle.x = -180 - angle.x;

        angle.y = 0;
        angle.z = 0;
        return angle;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rigidBody.velocity;
        velocity.x *= 1 - (horizontalDrag * Time.fixedDeltaTime);
        velocity.y *= 1 - (horizontalDrag * Time.fixedDeltaTime);
        velocity.z *= 1 - (verticalDrag * Time.fixedDeltaTime);
        rigidBody.velocity = velocity;
        
        UpdateMovement();
    }
    
    private void UpdateMovement()
    {
        bool canJump = CanJump();
        float movementSpeed = canJump ? groundMovementSpeed : airMovementSpeed;
        Vector3 moveDirection = new Vector3();

        if (Input.GetKey(moveForwardKey))
            moveDirection += transform.forward;

        if (Input.GetKey(moveBackwardKey))
            moveDirection -= transform.forward;

        if (Input.GetKey(moveRightKey))
            moveDirection += transform.right;

        if (Input.GetKey(moveLeftKey))
            moveDirection -= transform.right;

        moveDirection.Normalize();
        moveDirection *= movementSpeed * Time.fixedDeltaTime;

        Vector3 velocity = rigidBody.velocity;
        velocity.x += moveDirection.x;
        velocity.z += moveDirection.z;

        if (Input.GetKey(jumpKey) && canJump)
            velocity.y = jumpVelocity;

        rigidBody.velocity = velocity;
    }

    private bool CanJump()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        return Physics.Raycast(ray, 4.5f);
    }
}