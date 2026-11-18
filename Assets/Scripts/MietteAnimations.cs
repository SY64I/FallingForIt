using UnityEngine;

public class MietteAnimations : MonoBehaviour
{
    // Note: This script's functionality will likely be intergrated into the enemy AI acript once it's complete.

    [SerializeField] Animator anim;
    [SerializeField] Transform playerGrabLocation;

    [SerializeField] Rigidbody rb;
    [SerializeField] float walkingThreshold;

    bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.3f;
    [SerializeField] LayerMask groundMask;

    [SerializeField] EnemyBehavior oldMonsterAI;
    bool playerGrabbed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gravity = Physics.gravity;
        if (gravity.sqrMagnitude < 0.001f) return;

        Vector3 up = -gravity.normalized;

        isGrounded = CheckGrounded(up);

        anim.SetBool("isGrounded", isGrounded);

        // Likely can use a better method of deteriming if the monster is moving later on
        if (rb.linearVelocity.sqrMagnitude > walkingThreshold)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerGrabbed)
        {
            rb.linearVelocity = Vector3.zero;
            other.transform.parent = playerGrabLocation;
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.Euler(0, 180, 0);
            other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            oldMonsterAI.enabled = false;
            anim.SetTrigger("attack");
            playerGrabbed = true;
        }
    }
}
