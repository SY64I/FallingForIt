using NUnit.Framework;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private ButtonController buttonController;
    public GameObject door;

    // Update is called once per frame
    void Update()
    {
        if (buttonController.buttonPressed)
        {
            door.SetActive(false);
        }
        else
        {
            door.SetActive(true);
        }
    }
}
