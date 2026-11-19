using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float stopDistance = 1.5f;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 8f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector3 gravityDirection = Physics.gravity.sqrMagnitude > 0.001f ? Physics.gravity.normalized : Vector3.down;

        Vector3 upDirection = -gravityDirection;

        Vector3 toPlayer = Vector3.ProjectOnPlane(player.position - rb.position, upDirection);

        float distance = toPlayer.magnitude;

        bool shouldChase = distance <= detectionRange && distance > stopDistance;

        Vector3 desiredMovement = Vector3.zero;
        Vector3 forwardDirection;

        if (shouldChase)
        {
            forwardDirection = toPlayer.normalized;
            desiredMovement = forwardDirection * moveSpeed;
        }
        else
        {
            forwardDirection = Vector3.ProjectOnPlane(rb.rotation * Vector3.forward, upDirection).normalized;
        }

        Vector3 gravityVelocity = Vector3.Project(rb.linearVelocity, gravityDirection);

        rb.linearVelocity = desiredMovement + gravityVelocity;

        if (forwardDirection.sqrMagnitude < 0.001f)
        {
            Vector3 rightDirection = Vector3.ProjectOnPlane(rb.rotation * Vector3.right, upDirection);

            if (rightDirection.sqrMagnitude > 0.001f)
            {
                forwardDirection = Vector3.Cross(rightDirection.normalized, upDirection);
            }
        }

        if (forwardDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, upDirection);

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (player == null)
            return;

        if (collision.transform == player || collision.transform.IsChildOf(player))
        {
            Debug.Log("The player has died");
        }
    }
}
