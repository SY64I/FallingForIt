using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        Search
    }

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsule;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform navigationPivot;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform eyePoint;

    [SerializeField] private Transform player;
    [SerializeField] private GravityNavMeshManager navMeshManager;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 20f;

    [Tooltip("Field of view used to first notice the player")]
    [SerializeField, Range(10f, 360f)] private float viewAngle = 160f;

    [SerializeField] private float destinationUpdateInterval = 0.25f;
    [SerializeField] private float targetSampleDistance = 3f;
    [SerializeField] private float fallbackSampleDistance = 25f;
    [SerializeField] private float playerTargetHeight = 1f;

    [Tooltip("Environment geometry used for the ground check.")]
    [SerializeField] private LayerMask environmentMask;

    [Tooltip("Layers that block line of sight. Level geometry only.")]
    [SerializeField] private LayerMask sightBlockers;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolWaitTime = 1.5f;
    [SerializeField] private float patrolWanderRadius = 8f;
    [SerializeField] private float minWanderDistance = 2f;

    [Header("Chase & Search")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float searchDuration = 4f;
    [SerializeField] private float arriveDistance = 0.5f;

    [Header("Movement")]
    [SerializeField] private float idleDeceleration = 15f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float directChargeRange = 6f;

    [Header("Ledges")]
    [SerializeField] private float maxDropHeight = 4f;
    [SerializeField] private float airborneDetachDelay = 0.25f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float navMeshSnapDistance = 0.75f;

    private State state = State.Patrol;

    private bool grounded;
    private bool agentRegistered;
    private float nextBehaviorUpdate;

    private float airborneTime;

    private Vector3 lastKnownPlayerPosition;

    private int patrolIndex = -1;
    private bool hasPatrolDestination;
    private float patrolWaitUntil;

    private bool searchWaiting;
    private float searchEndTime;

    private NavMeshPath pathCheck;

    private void Awake()
    {
        pathCheck = new NavMeshPath();

        if (navMeshManager == null)
            navMeshManager = FindFirstObjectByType<GravityNavMeshManager>();

        // Guards against a prefab asset (or nothing) being assigned instead of the scene player
        if (player == null || !player.gameObject.scene.IsValid())
        {
            PlayerControls playerControls = FindFirstObjectByType<PlayerControls>();
            player = playerControls != null ? playerControls.transform : null;
        }

        if (rb == null) rb = GetComponent<Rigidbody>();
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();

        if (rb == null || capsule == null)
        {
            Debug.LogError("Enemy requires a Rigidbody and CapsuleCollider.");
            enabled = false;
            return;
        }

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (agent == null || navigationPivot == null)
        {
            Debug.LogError("Assign the NavMeshAgent and NavigationPivot!");
            enabled = false;
            return;
        }

        // The NavMeshAgent only plans paths; the Rigidbody does the moving
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.areaMask = NavMesh.AllAreas;

        // Registers itself once the enemy has landed on the active NavMesh
        agent.enabled = false;
    }

    private void OnEnable()
    {
        if (navMeshManager != null)
        {
            navMeshManager.GravityChanging += OnGravityChanging;
        }
    }

    private void OnDisable()
    {
        if (navMeshManager != null)
        {
            navMeshManager.GravityChanging -= OnGravityChanging;
        }
    }

    private void Update()
    {
        if (navMeshManager == null || !navMeshManager.IsInitialized)
            return;
        
        if (agentRegistered && airborneTime > airborneDetachDelay)
        {
            DetachAgent(false);
            return;
        }

        if (!agentRegistered)
        {
            if (grounded)
                TryRegisterOnNavMesh();

            return;
        }

        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        agent.nextPosition = navigationPivot.position;

        if (Time.time >= nextBehaviorUpdate)
        {
            UpdateBehavior();
            nextBehaviorUpdate = Time.time + destinationUpdateInterval;
        }
    }

    private void FixedUpdate()
    {
        if (Physics.gravity.sqrMagnitude < 0.001f)
            return;

        rb.angularVelocity = Vector3.zero;

        Vector3 gravityDirection = Physics.gravity.normalized;
        Vector3 up = -gravityDirection;

        grounded = CheckGrounded(gravityDirection);

        airborneTime = grounded ? 0f : airborneTime + Time.fixedDeltaTime;

        MoveEnemy(up, gravityDirection);
    }

    // Behavior

    private void UpdateBehavior()
    {
        bool seesPlayer = player != null && CanSeePlayer();

        if (seesPlayer)
            lastKnownPlayerPosition = player.position;

        switch (state)
        {
            case State.Patrol:
                UpdatePatrol(seesPlayer);
                break;

            case State.Chase:
                UpdateChase(seesPlayer);
                break;

            case State.Search:
                UpdateSearch(seesPlayer);
                break;
        }

        agent.speed = state == State.Patrol ? patrolSpeed : chaseSpeed;
    }

    private void UpdatePatrol(bool seesPlayer)
    {
        if (seesPlayer)
        {
            EnterChase();
            return;
        }

        // Pausing at a waypoint
        if (Time.time < patrolWaitUntil)
            return;

        // Walking to the current destination
        if (hasPatrolDestination)
        {
            if (!Arrived())
                return;

            // Arrived (stand still for a moment)
            hasPatrolDestination = false;
            agent.isStopped = true;
            patrolWaitUntil = Time.time + patrolWaitTime;
            return;
        }

        if (TryPickPatrolDestination(out Vector3 destination))
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
            hasPatrolDestination = true;
        }
        else
        {
            // Try again after a pause
            agent.isStopped = true;
            patrolWaitUntil = Time.time + patrolWaitTime;
        }
    }

    private void UpdateChase(bool seesPlayer)
    {
        if (!seesPlayer)
        {
            EnterSearch();
            return;
        }

        if (!TrySetDestination(player.position))
            agent.isStopped = true;
    }

    private void UpdateSearch(bool seesPlayer)
    {
        if (seesPlayer)
        {
            EnterChase();
            return;
        }

        if (!searchWaiting)
        {
            // Still walking to where the player was last seen
            if (Arrived())
            {
                searchWaiting = true;
                searchEndTime = Time.time + searchDuration;
                agent.isStopped = true;
            }

            return;
        }

        if (Time.time >= searchEndTime)
            EnterPatrol();
    }

    private void EnterPatrol()
    {
        state = State.Patrol;
        hasPatrolDestination = false;
        patrolWaitUntil = 0f;
        searchWaiting = false;
    }

    private void EnterChase()
    {
        state = State.Chase;
        hasPatrolDestination = false;
        searchWaiting = false;

        UpdateChase(true);
    }

    private void EnterSearch()
    {
        state = State.Search;
        searchWaiting = false;

        // The last known position may no longer be on the active NavMesh
        if (!TrySetDestination(lastKnownPlayerPosition))
            EnterPatrol();
    }

    private bool TryPickPatrolDestination(out Vector3 destination)
    {
        // Fixed route first
        if (patrolPoints != null)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                Transform point = patrolPoints[patrolIndex];

                if (point != null &&
                    NavMesh.SamplePosition(point.position, out NavMeshHit pointHit, targetSampleDistance, Filter()) &&
                    IsReachable(pointHit.position))
                {
                    destination = pointHit.position;
                    return true;
                }
            }
        }

        // Otherwise wander around
        Vector3 up = -Physics.gravity.normalized;

        for (int attempt = 0; attempt < 8; attempt++)
        {
            Vector3 direction = Vector3.ProjectOnPlane(Random.onUnitSphere, up).normalized;
            float distance = Random.Range(minWanderDistance, patrolWanderRadius);
            Vector3 candidate = navigationPivot.position + direction * distance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, Filter()) &&
                Vector3.Distance(hit.position, navigationPivot.position) >= minWanderDistance &&
                IsReachable(hit.position))
            {
                destination = hit.position;
                return true;
            }
        }

        destination = Vector3.zero;
        return false;
    }

    private bool IsReachable(Vector3 target)
    {
        return agent.CalculatePath(target, pathCheck) &&
               pathCheck.status == NavMeshPathStatus.PathComplete;
    }

    private bool TrySetDestination(Vector3 worldPosition)
    {
        // Find the closest point
        bool found =
            NavMesh.SamplePosition(worldPosition, out NavMeshHit hit, targetSampleDistance, Filter()) ||
            NavMesh.SamplePosition(worldPosition, out hit, fallbackSampleDistance, Filter());

        if (!found)
            return false;

        agent.isStopped = false;
        agent.SetDestination(hit.position);
        return true;
    }

    private bool Arrived()
    {
        return !agent.pathPending &&
               (!agent.hasPath || agent.remainingDistance <= arriveDistance);
    }

    // Movement

    private bool CheckGrounded(Vector3 gravityDirection)
    {
        if (groundCheck == null)
            return false;

        // Above the feet so the cast isn't already inside the floor
        Vector3 origin = groundCheck.position - gravityDirection * (groundCheckRadius + 0.05f);

        return Physics.SphereCast(
            origin,
            groundCheckRadius,
            gravityDirection,
            out _,
            0.05f + groundCheckDistance,
            environmentMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void MoveEnemy(Vector3 up, Vector3 gravityDirection)
    {
        Vector3 planarVelocity;

        if (agentRegistered && agent.enabled && agent.isOnNavMesh)
        {
            if (state == State.Chase && ShouldChargeDirectly(up))
            {
                Vector3 toPlayer = Vector3.ProjectOnPlane(player.position - transform.position, up).normalized;

                planarVelocity = GroundAhead(toPlayer, gravityDirection)
                    ? toPlayer * chaseSpeed
                    : Vector3.zero;
            }
            else
            {
                planarVelocity = Vector3.ProjectOnPlane(agent.desiredVelocity, up);
            }
        }
        else
        {
            // No active path (gradually remove horizontal movement)
            planarVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, up);
            if (grounded)
            {
                planarVelocity = Vector3.MoveTowards(
                    planarVelocity,
                    Vector3.zero,
                    idleDeceleration * Time.fixedDeltaTime
                );
            }
        }

        // Preserve the velocity along the gravity axis
        Vector3 verticalVelocity = Vector3.Project(rb.linearVelocity, gravityDirection);

        if (grounded)
        {
            float downwardSpeed = Vector3.Dot(verticalVelocity, gravityDirection);

            if (downwardSpeed > 0f)
                verticalVelocity -= gravityDirection * downwardSpeed;
        }

        rb.linearVelocity = planarVelocity + verticalVelocity;
        rb.AddForce(Physics.gravity, ForceMode.Acceleration);

        // Face the walking direction, or the player when chasing but standing still
        Vector3 faceDirection = planarVelocity;

        if (faceDirection.sqrMagnitude < 0.05f && state == State.Chase && player != null)
            faceDirection = Vector3.ProjectOnPlane(player.position - transform.position, up);

        Face(faceDirection, up);
    }

    private void Face(Vector3 direction, Vector3 up)
    {
        Vector3 forward = direction.sqrMagnitude > 0.05f
            ? direction
            : Vector3.ProjectOnPlane(rb.rotation * Vector3.forward, up);

        // Facing straight along the new up right after a shift
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.ProjectOnPlane(rb.rotation * Vector3.up, up);

        if (forward.sqrMagnitude < 0.001f)
            return;

        Quaternion target = Quaternion.LookRotation(forward.normalized, up);

        rb.MoveRotation(Quaternion.RotateTowards(
            rb.rotation,
            target,
            rotationSpeed * Time.fixedDeltaTime
        ));
    }

    // Walk the last stretch straight at the player
    private bool ShouldChargeDirectly(Vector3 up)
    {
        if (player == null)
            return false;

        Vector3 toPlayer = Vector3.ProjectOnPlane(player.position - transform.position, up);

        if (toPlayer.magnitude > directChargeRange)
            return false;

        bool agentStuck =
            agent.isStopped ||
            agent.pathStatus != NavMeshPathStatus.PathComplete ||
            agent.desiredVelocity.sqrMagnitude < 0.01f;

        return agentStuck && CanSeePlayer();
    }

    // Step off a ledge only when another surface is within maxDropHeight
    private bool GroundAhead(Vector3 direction, Vector3 gravityDirection)
    {
        const float probeHeight = 0.5f;

        Vector3 probe = navigationPivot.position + direction * 0.8f - gravityDirection * probeHeight;

        return Physics.Raycast(
            probe,
            gravityDirection,
            probeHeight + maxDropHeight,
            environmentMask,
            QueryTriggerInteraction.Ignore
        );
    }

    // Viewing

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > detectionRange)
            return false;

        if (state != State.Chase &&
            viewAngle < 360f &&
            Vector3.Angle(transform.forward, toPlayer) > viewAngle * 0.5f)
        {
            return false;
        }

        Vector3 origin = eyePoint != null ? eyePoint.position : transform.position;
        Vector3 up = -Physics.gravity.normalized;

        // Aim at the chest so the ray doesn't end on the floor under the player
        Vector3 targetPosition = player.position + up * playerTargetHeight;

        Vector3 direction = targetPosition - origin;
        float distance = direction.magnitude;

        if (distance <= 0.01f)
            return true;

        // Fall back to the environment layers if empty mask
        LayerMask blockers = sightBlockers.value != 0 ? sightBlockers : environmentMask;

        return !Physics.Raycast(
            origin,
            direction.normalized,
            distance,
            blockers,
            QueryTriggerInteraction.Ignore
        );
    }

    // NavMesh registration and gravity changes

    private bool TryRegisterOnNavMesh()
    {
        if (agentRegistered || navMeshManager == null)
            return false;

        if (!grounded || !navMeshManager.IsInitialized)
            return false;

        if (!NavMesh.SamplePosition(
            navigationPivot.position,
            out NavMeshHit hit,
            navMeshSnapDistance,
            Filter()))
        {
            return false;
        }

        Vector3 savedPosition = agent.transform.position;

        agent.enabled = true;

        if (!agent.Warp(hit.position))
        {
            agent.enabled = false;
            return false;
        }

        agent.transform.position = savedPosition;

        if (agent.transform == transform)
            rb.position = savedPosition;

        agent.nextPosition = hit.position;
        agent.isStopped = true;

        agentRegistered = true;

        return true;
    }

    private void OnGravityChanging()
    {
        // Detach from the old NavMesh before it is disabled
        DetachAgent(true);

        grounded = false;

        airborneTime = 0f;

        // Nothing from the old orientation carries over
        rb.linearVelocity = Vector3.zero;
    }

    // Reset behavior on gravity changes; false when falling off a ledge.
    private void DetachAgent(bool resetBehavior)
    {
        if (agent.enabled)
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        agentRegistered = false;

        // Clear destination and waiting state
        hasPatrolDestination = false;
        patrolWaitUntil = 0f;
        searchWaiting = false;

        // On a gravity change, reset to Patrol
        if (resetBehavior || state == State.Search)
            EnterPatrol();

        nextBehaviorUpdate = 0f;
    }

    private NavMeshQueryFilter Filter()
    {
        return new NavMeshQueryFilter
        {
            agentTypeID = agent.agentTypeID,
            areaMask = NavMesh.AllAreas
        };
    }
}