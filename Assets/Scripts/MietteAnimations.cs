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

    [SerializeField] EnemyAI monsterAI;
    [SerializeField] private AudioClip[] mietteIdleSoundClips;
    [SerializeField] private AudioClip[] mietteChaseSoundClips;
    public float counterTime = 6f;
    bool playerGrabbed;
    int mietteState = 0; //0 for idle, 1 for stuck (not implemented yet), 2 for chase

    [SerializeField] private Transform player;
    PlayerControls playerControls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControls = player.GetComponent<PlayerControls>();
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
            mietteState = 2;
        }
        else
        {
            anim.SetBool("isWalking", false);
            mietteState = 0;
        }

        if (playerGrabbed && !GameManager.instance.respawning)
        {
            player.transform.localPosition = Vector3.zero;
        }
    

        if (counterTime > 0)
        {
            counterTime -= Time.deltaTime;
        }
        else
        {
            if (mietteState == 0) // idle
            {
                int randomSound = Random.Range(0, mietteIdleSoundClips.Length);
                var clip = mietteIdleSoundClips[randomSound];
                SoundFXManager.instance.PlaySpatialSoundFXClip(clip, transform, 1f);
            } 
            else if (mietteState == 2) // chasing
            {
                int randomSound = Random.Range(0, mietteChaseSoundClips.Length);
                var clip = mietteChaseSoundClips[randomSound];
                SoundFXManager.instance.PlaySpatialSoundFXClip(clip, transform, 1f);            
            }
            // randomize time until next sound
            int time = Random.Range(3, 10);
            counterTime = time;
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
        if (other.CompareTag("Player"))
        {
            if (!playerGrabbed && !playerControls.GodMode)
            {
                rb.linearVelocity = Vector3.zero;
                playerControls.DeathByMonster();
                player.parent = playerGrabLocation;
                player.localRotation = Quaternion.Euler(0, 180, 0);
                monsterAI.enabled = false;
                anim.SetTrigger("attack");
                playerGrabbed = true;
            }

        }
    }

    public void Respawn()
    {
        rb.position = GameManager.instance.monsterSpawnPoint;
        playerGrabbed = false;
        playerGrabLocation.DetachChildren();
        monsterAI.enabled = true;
        anim.Play("Idle", 0, 0f);
    }
}
