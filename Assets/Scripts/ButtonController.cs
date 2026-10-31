using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
    [SerializeField] Animator buttonAnim;
    public bool buttonPressed;
    private bool playerInRange;

    void Start()
    {
        buttonPressed = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            // player in range to press button
            Debug.Log("in button range");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Player")
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
        buttonPressed = !buttonPressed;
        if (buttonPressed) // button is pressed
        {
            buttonAnim.SetBool("buttonPressed", buttonPressed);
            Debug.Log("button is pressed");
        }
        else // button is unpressed
        {
            buttonAnim.SetBool("buttonPressed", buttonPressed);
            Debug.Log("button is unpressed");
        }
        
    }

}
