using UnityEngine;
using UnityEngine.InputSystem;

public class GravityController : MonoBehaviour
{
    [SerializeField] private GravityDirection selector;
    [SerializeField] private float gravityStrength = 2f;
    [SerializeField] private PlayerControls playerController;
    [SerializeField] private AudioClip gravitySoundClip;
    [SerializeField] private GravityNavMeshManager navMeshManager;

    private const float BaseGravity = 9.81f;

    private void Start()
    {
        ResetGravityDirection();

        if (navMeshManager != null)
        {
            navMeshManager.InitializeForGravity(Physics.gravity);
        }
        else
        {
            Debug.LogError("GravityNavMeshManager is not assigned!");
        }
    }

    private void Update()
    {
        bool playerIsDead = playerController != null && playerController.isDead;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            !playerIsDead)
        {
            ShiftGravity();
        }
    }

    public void ShiftGravity()
    {
        if (selector == null)
        {
            Debug.LogError("GravityDirection selector is not assigned!");
            return;
        }

        if (!selector.GetGravityDirection(out Vector3 newGravity))
            return;

        if (SoundFXManager.instance != null && gravitySoundClip != null)
            SoundFXManager.instance.PlaySoundFXClip(gravitySoundClip, transform, 1f);

        // Gravity must change before the NavMesh manager is told,
        // because enemies read Physics.gravity when they detach
        Physics.gravity = newGravity * BaseGravity * gravityStrength;

        if (navMeshManager != null)
            navMeshManager.SetGravityDirection(Physics.gravity);

        if (playerController != null)
            playerController.AlignPlayerToGravity();
    }

    public void ResetGravityDirection()
    {
        Physics.gravity = Vector3.down * BaseGravity * gravityStrength;
    }
}