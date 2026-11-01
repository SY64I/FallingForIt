using NUnit.Framework;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Switch[] connectedSwitches;
    private int numberOfSwitches;
    public GameObject door;

    private void Start()
    {
        numberOfSwitches = connectedSwitches.Length;
    }

    // Update is called once per frame
    void Update()
    {
        // NOTE: I've set this up with a predefinte amount of switch amounts due to my plans to add door lights to the door models to indicate how many switches need to be pressed
        switch (numberOfSwitches)
        {
            case 1:
                if (connectedSwitches[0].activated)
                {
                    door.SetActive(false);
                }
                else door.SetActive(true);

                break;
            case 2:
                if (connectedSwitches[0].activated && connectedSwitches[1].activated)
                {
                    door.SetActive(false);
                }
                else door.SetActive(true);
                break;
            case 3:
                if (connectedSwitches[0].activated && connectedSwitches[1].activated && connectedSwitches[2].activated)
                {
                    door.SetActive(false);
                }
                else door.SetActive(true);
                break;
            case 4:
                if (connectedSwitches[0].activated && connectedSwitches[1].activated && connectedSwitches[2].activated && connectedSwitches[3].activated)
                {
                    door.SetActive(false);
                }
                else door.SetActive(true);
                break;
            default:
                break;
        }
    }
}
