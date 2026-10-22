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

    [SerializeField] Animator playerAnim;

    Vector3 playerVelocity;
    bool isGrounded;

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
        // Check if player is touching the ground
        isGrounded = controller.isGrounded;
        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpPressedTime = Time.time;
        }
        // Jump
        if (isGrounded && Time.time - jumpPressedTime <= jumpBufferTime)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpPressedTime = float.NegativeInfinity;

            playerAnim.SetTrigger("jump");
        }
        /*
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );
        */

        //if (isGrounded) { Debug.Log("grounded"); }
        //else { Debug.Log("in air"); }

        // WASD movement
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Get camera directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ignore camera's vertical tilt
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Camera-relative movement
        Vector3 movement = cameraForward * input.y + cameraRight * input.x;
        movement = Vector3.ClampMagnitude(movement, 1f);

        // Rotate player to face movement direction
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            playerAnim.SetBool("running", true);
        }
        else
        {
            playerAnim.SetBool("running", false);
        }

        controller.Move(movement * playerSpeed * Time.deltaTime);

        // Keep player on the ground
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // Gravity
        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);

        playerAnim.SetBool("isGrounded", controller.isGrounded);
    }
}