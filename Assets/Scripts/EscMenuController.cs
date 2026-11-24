using UnityEngine;
using UnityEngine.InputSystem;


public class EscMenuController : MonoBehaviour
{
   [SerializeField] private GameObject escapeMenu;
    //public CameraController cam;

   bool escapeMenuActive = false;


   private void Update()
   {
       if (Keyboard.current != null &&
           Keyboard.current.escapeKey.wasPressedThisFrame)
       {
            escapeMenuActive = !escapeMenuActive;
            if (escapeMenuActive) // escape menu active
            {
                Time.timeScale = 0f;
                escapeMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else // escape menu not active
            {
                escapeMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
       }
   }
}