using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Switch))]
public class ButtonController : MonoBehaviour
{
    [SerializeField] Animator buttonAnim;
    [SerializeField] MeshRenderer buttonActivatorMesh;
    [SerializeField] Material buttonOffMat, buttonOnMat;
    private Switch switchState;
    private bool playerInRange;

    void Start()
    {
        switchState = GetComponent<Switch>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            // player in range to press button
            Debug.Log("in button range");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            // player is out of range to press button
            Debug.Log("out of button range");
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame) // if player is within range and pressing interact key
        {
            ToggleButton();
        }
        
    }

    private void ToggleButton()
    {
        switchState.activated = !switchState.activated;
        buttonAnim.SetBool("buttonPressed", switchState.activated);

        if (switchState.activated) // button is pressed
        {
            buttonActivatorMesh.material = buttonOnMat;
            Debug.Log("button is pressed");
        }
        else // button is unpressed
        {
            buttonActivatorMesh.material = buttonOffMat;
            Debug.Log("button is unpressed");
        }
        
    }

}
