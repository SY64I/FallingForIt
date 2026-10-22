using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    [SerializeField] private GravityDirection selector;
    [SerializeField] private float gravityStrength = 9.81f;
    [SerializeField] private PlayerControls playerController;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key detected!");
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

    Physics.gravity = newGravity * gravityStrength;

    if (playerController != null)
    {
        playerController.AlignPlayerToGravity();
    }

    Debug.Log("New gravity: " + Physics.gravity);
    Debug.Log("GravityController successfully applied gravity.");
    }
}