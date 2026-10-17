using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float playerSpeed = 8f;
    [SerializeField] float gravity = -50f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpBufferTime = 0.12f;
    float jumpPressedTime = float.NegativeInfinity;

    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;

    [SerializeField] Transform cameraTransform;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundMask;

    bool isGrounded;

    private Vector3 gravityVelocity = Vector3.zero;

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        Vector3 up = -Physics.gravity.normalized;

        isGrounded = controller.isGrounded;

        if (jumpAction.action.WasPressedThisFrame())
            jumpPressedTime = Time.time;

        // Stop accumulating gravity while grounded
        if (isGrounded && Vector3.Dot(gravityVelocity, up) < 0f)
            gravityVelocity = -up * 2f;

        // Jump along "up", written into gravityVelocity (the one actually applied)
        if (isGrounded && Time.time - jumpPressedTime <= jumpBufferTime)
        {
            gravityVelocity = up * Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude);
            jumpPressedTime = float.NegativeInfinity;
        }

        if (isGrounded) { Debug.Log("grounded"); }
        else { Debug.Log("in air"); }

        // WASD movement
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, up);
        Vector3 cameraRight   = Vector3.ProjectOnPlane(cameraTransform.right, up);

        // Fallback when looking straight along the gravity axis
        if (cameraForward.sqrMagnitude < 0.001f)
            cameraForward = Vector3.ProjectOnPlane(cameraTransform.up, up);

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Camera-relative movement
        Vector3 movement = cameraForward * input.y + cameraRight * input.x;
        movement = Vector3.ClampMagnitude(movement, 1f);

        // Rotate player to face movement direction
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Apply gravity in the world's current gravity direction
        gravityVelocity += Physics.gravity * Time.deltaTime;

        // Combine player movement and gravity into one movement call
        Vector3 finalMovement = movement * playerSpeed + gravityVelocity;

        controller.Move(finalMovement * Time.deltaTime);
    }

    public void AlignPlayerToGravity()
    {
        if (Physics.gravity.sqrMagnitude < 0.001f) return;

        Vector3 newUp = -Physics.gravity.normalized;

        // Drop momentum from the old gravity direction
        gravityVelocity = Vector3.zero;

        // Handle a full 180° flip, where FromToRotation is ambiguous
        Quaternion delta = Vector3.Dot(transform.up, newUp) < -0.999f
            ? Quaternion.AngleAxis(180f, transform.forward)
            : Quaternion.FromToRotation(transform.up, newUp);

        transform.rotation = delta * transform.rotation;
    }
}