using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float playerSpeed = 8f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpBufferTime = 0.12f;
    float jumpPressedTime = float.NegativeInfinity;

    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;

    [SerializeField] Transform cameraTransform;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.3f;
    [SerializeField] LayerMask groundMask;

    Vector3 playerVelocity;
    [SerializeField] Animator playerAnim;
    
    [SerializeField] Vector3 spawnPoint;

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

    Vector2 moveInput;

    void Update()
    {
        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpPressedTime = Time.time;
            Debug.Log("JUMP BUTTON PRESSED");
        }

        moveInput = moveAction.action.ReadValue<Vector2>();
    }


    void FixedUpdate()
    {
        // Prevent physics collisions from making the player spin
        rb.angularVelocity = Vector3.zero;
        
        Vector3 gravity = Physics.gravity;
        if (gravity.sqrMagnitude < 0.001f) return;

        Vector3 up = -gravity.normalized;

        isGrounded = CheckGrounded(up);

        Vector3 velocity = rb.linearVelocity;
        float verticalSpeed = Vector3.Dot(velocity, up);

        // Stop downward movement while grounded
        if (isGrounded && verticalSpeed < 0f)
        {
            velocity -= up * verticalSpeed;
        }

        // Jump when grounded and the button was recently pressed
        if (isGrounded &&
            Time.time - jumpPressedTime <= jumpBufferTime)
        {
            Debug.Log("JUMP TRIGGERED");

            float jumpSpeed =
                Mathf.Sqrt(2f * jumpHeight * gravity.magnitude);

            velocity += up * jumpSpeed;

            jumpPressedTime = float.NegativeInfinity;
            playerAnim.SetTrigger("jump");
        }

        // Camera-relative movement
        Vector3 cameraForward =
            Vector3.ProjectOnPlane(cameraTransform.forward, up).normalized;

        Vector3 cameraRight =
            Vector3.ProjectOnPlane(cameraTransform.right, up).normalized;

        if (cameraForward.sqrMagnitude < 0.001f)
        {
            cameraForward =
                Vector3.ProjectOnPlane(cameraTransform.up, up).normalized;
        }

        Vector3 movement =
            cameraForward * moveInput.y + cameraRight * moveInput.x;

        movement = Vector3.ClampMagnitude(movement, 1f);

        // Rotate toward movement direction
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movement, up);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );

            playerAnim.SetBool("running", true);
        }
        else
        {
            playerAnim.SetBool("running", false);
        }

        // Set movement while preserving velocity along gravity
        Vector3 horizontalVelocity =
            movement * playerSpeed;

        Vector3 verticalVelocity =
            Vector3.Project(velocity, up);
        
        //Debug.Log("Velocity: " + rb.linearVelocity);

        rb.linearVelocity = horizontalVelocity + verticalVelocity;

        // Apply gravity exactly once
        rb.AddForce(gravity, ForceMode.Acceleration);

        playerAnim.SetBool("isGrounded", isGrounded);
    }

    public void AlignPlayerToGravity()
    {
        if (Physics.gravity.sqrMagnitude < 0.001f)
            return;

        Vector3 newUp = -Physics.gravity.normalized;

        // Rotate the player to match the new gravity direction
        Quaternion delta =
            Vector3.Dot(transform.up, newUp) < -0.999f
            ? Quaternion.AngleAxis(180f, transform.forward)
            : Quaternion.FromToRotation(transform.up, newUp);

        rb.rotation = delta * rb.rotation;
    }

    bool CheckGrounded(Vector3 up)
    {
        return Physics.Raycast(
            groundCheck.position,
            -up,
            groundDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goo"))
        {
            enabled = false;
            playerAnim.SetTrigger("dieAcid");
            StartCoroutine(this.deathByAcid());
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            enabled = false;
            playerAnim.SetTrigger("dieMonster");
            StartCoroutine(this.deathByAcid());
        }
    }

    private IEnumerator deathByAcid()
    {
        Debug.Log("Player has collided with Goo");
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(1.2f);
        this.playerDies();
    }

    private void playerDies()
    {
        Debug.Log("Player has died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        /*
        rb.position = spawnPoint;
        // Cancel the death animation
        playerAnim.Play("Idle", 0, 0f);
        enabled = true;
        */
    }
}