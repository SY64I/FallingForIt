using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CapsuleCollider playerCollision;
    [SerializeField] Rigidbody rb;
    [SerializeField] float playerSpeed = 8f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpBufferTime = 0.12f;
    float jumpPressedTime = float.NegativeInfinity;

    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;

    [SerializeField] Transform cameraTransform;
    [SerializeField] private Transform rotationPivot;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.3f;
    [SerializeField] LayerMask groundMask;

    Vector3 playerVelocity;
    [SerializeField] Animator playerAnim;
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] private AudioClip[] landingSoundClip;
    [SerializeField] private AudioClip acidSoundClip;
    public bool GodMode = false;

    bool canMove;
    bool isGrounded;

    private Vector3 gravityVelocity = Vector3.zero;

    public bool isDead = false;
    bool hasDied = false;

    [HideInInspector] public float edgeHeightAmount;
    public float edgeHeightMax = 1.5f;
    [SerializeField] float edgeHeightMin = -2f;
    [SerializeField] float edgeHeightChangeSpeed = 1f;
    [HideInInspector] public bool diedToGoo;
    bool diedToFalling;

    [SerializeField] GameObject camRigObj;

    private Vector3 pivotLocalOffset;
    private float pivotToFeetDistance;

    private void Awake()
    {
        Transform body = rb != null ? rb.transform : transform;

        Vector3 pivotWorld;

        if (rotationPivot != null)
            pivotWorld = rotationPivot.position;
        else if (playerCollision != null)
            pivotWorld = body.TransformPoint(playerCollision.center);   // fall back to the collider's center
        else
            pivotWorld = body.position;

        pivotLocalOffset = body.InverseTransformPoint(pivotWorld);

        if (groundCheck != null)
            pivotToFeetDistance = Vector3.Distance(pivotWorld, groundCheck.position);
    }

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

    private void Start()
    {
        edgeHeightAmount = edgeHeightMax;
    }

    void Update()
    {
        if (!isDead)
        {
            if (jumpAction.action.WasPressedThisFrame())
            {
                jumpPressedTime = Time.time;
                Debug.Log("JUMP BUTTON PRESSED");
            }

            moveInput = moveAction.action.ReadValue<Vector2>();
        }
        else
        {
            if (diedToGoo)
            {
                if (edgeHeightAmount > edgeHeightMin)
                {
                    edgeHeightAmount -= Time.deltaTime * edgeHeightChangeSpeed;
                }
            }
        }
        if (isDead && !hasDied)
        {
            Debug.Log("Player has died");
            if (!diedToFalling)
            {
                rb.isKinematic = true;
                playerCollision.enabled = false;
                rb.linearVelocity = Vector3.zero;
            }
            GameManager.instance.StartCoroutine(GameManager.instance.DeathSequence());
            hasDied = true;
        }


    }
    void FixedUpdate()
    {
        if (!isDead)
        {
            bool oldGrounded = isGrounded;

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

            rb.linearVelocity = horizontalVelocity + verticalVelocity;

            // Apply gravity exactly once
            rb.AddForce(gravity, ForceMode.Acceleration);

            playerAnim.SetBool("isGrounded", isGrounded);

            if ((oldGrounded == false) && (isGrounded == true))
            {
                // just landed on floor, play sound
                int random = Random.Range(0, landingSoundClip.Length);
                var clip = landingSoundClip[random];
                SoundFXManager.instance.PlaySoundFXClip(clip, transform, 1f);
            }
        }
    }

    public void AlignPlayerToGravity()
    {
        if (Physics.gravity.sqrMagnitude < 0.001f)
            return;

        Vector3 newUp = -Physics.gravity.normalized;

        Quaternion currentRotation = rb.rotation;
        Vector3 currentUp = currentRotation * Vector3.up;

        // Rotation needed to match the new gravity
        Quaternion delta =
            Vector3.Dot(currentUp, newUp) < -0.999f
            ? Quaternion.AngleAxis(180f, currentRotation * Vector3.forward)
            : Quaternion.FromToRotation(currentUp, newUp);

        Quaternion newRotation = delta * currentRotation;

        Vector3 scaledOffset = Vector3.Scale(pivotLocalOffset, rb.transform.lossyScale);
        Vector3 offsetBefore = currentRotation * scaledOffset;
        Vector3 offsetAfter = newRotation * scaledOffset;

        Vector3 pivotWorld = rb.position + offsetBefore;

        rb.position = pivotWorld - offsetAfter;
        rb.rotation = newRotation;

        // Pushes player along the new up if too close to new floor
        if (pivotToFeetDistance > 0f &&
            Physics.Raycast(
                pivotWorld,
                -newUp,
                out RaycastHit hit,
                pivotToFeetDistance,
                groundMask,
                QueryTriggerInteraction.Ignore))
        {
            rb.position += newUp * (pivotToFeetDistance - hit.distance + 0.02f);
        }
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
        if (!isDead && !GodMode)
        {
            if (collision.gameObject.CompareTag("Goo"))
            {
                playerAnim.SetTrigger("dieAcid");
                diedToGoo = true;
                isDead = true;
            }
        }
    }

    public void DeathByMonster()
    {
        if (!isDead)
        {
            playerAnim.SetTrigger("dieMonster");
            isDead = true;
        }
    }

    public void DeathByFalling()
    {
        if (!isDead)
        {
            playerAnim.SetTrigger("dieFall");
            camRigObj.SetActive(false);
            isDead = true;
            diedToFalling = true;
        }
    }

    public void Respawn()
    {
        transform.parent = null;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.position = GameManager.instance.playerSpawnPoint;
        AlignPlayerToGravity();
        //transform.position = GameManager.instance.playerSpawnPoint;
        // Cancel the death animation
        playerAnim.Play("Idle", 0, 0f);
        diedToGoo = false;
        diedToFalling = false;
        camRigObj.SetActive(true);
        edgeHeightAmount = edgeHeightMax;
        playerCollision.enabled = true;
        isDead = false;
        hasDied = false;
    }
}