using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    [SerializeField] private GravityDirection selector;
    private float gravity = 9.81f;
    [SerializeField] private float gravityStrength = 2f;
    [SerializeField] private PlayerControls playerController;
    [SerializeField] private AudioClip gravitySoundClip;

    private void Start()
    {
        ResetGravityDirection();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.leftShiftKey.wasPressedThisFrame && !playerController.isDead)
        {
            Debug.Log("shift key detected!");
            ShiftGravity();
        }
    }

    public void ShiftGravity()
    {
         Debug.Log("ShiftGravity started.");

        if (selector == null)
        {
            Debug.LogError("GravityDirection selector is not assigned!");
            return;
        }

        bool success = selector.GetGravityDirection(out Vector3 newGravity);

        Debug.Log("GetGravityDirection returned: " + success);

        if (!success)
        {
            Debug.LogWarning("Could not determine gravity direction.");
            return;
        }

        // play gravity sound effect
        SoundFXManager.instance.PlaySoundFXClip(gravitySoundClip, transform, 1f);
        
        Physics.gravity = newGravity * gravity * gravityStrength;

        if (playerController != null)
        {
            playerController.AlignPlayerToGravity();
        }

        Debug.Log("New gravity: " + Physics.gravity);
        Debug.Log("GravityController successfully applied gravity.");
    }

    public void ResetGravityDirection()
    {
        Physics.gravity = Vector3.down * gravity * gravityStrength;
    }
}