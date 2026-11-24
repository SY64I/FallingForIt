using NUnit.Framework;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Switch[] connectedSwitches;

    Animator anim;
    bool isOpen;
    private int numberOfSwitches;
    public GameObject door;
    // Note: This implementation assumes there are exactly 4 light parent objects.
    [SerializeField] GameObject[] lightParentObjects;
    // Note: This implementation assumes there are exactly 10 lights.
    [SerializeField] ActivatedLight[] lights;
    [SerializeField] private AudioClip doorSoundClip;

    [SerializeField] private bool showObjectiveOnOpen = false;

    private void Start()
    {
        numberOfSwitches = connectedSwitches.Length;
        lightParentObjects[0].SetActive(false);
        // Automatically set connected switches of lights
        switch (numberOfSwitches)
        {
            case 1:
                lightParentObjects[0].SetActive(true);
                lights[0].connectedSwitch = connectedSwitches[0];
                break;
            case 2:
                lightParentObjects[1].SetActive(true);
                lights[1].connectedSwitch = connectedSwitches[0];
                lights[2].connectedSwitch = connectedSwitches[1];
                break;
            case 3:
                lightParentObjects[2].SetActive(true);
                lights[3].connectedSwitch = connectedSwitches[0];
                lights[4].connectedSwitch = connectedSwitches[1];
                lights[5].connectedSwitch = connectedSwitches[2];
                break;
            case 4:
                lightParentObjects[3].SetActive(true);
                lights[6].connectedSwitch = connectedSwitches[0];
                lights[7].connectedSwitch = connectedSwitches[1];
                lights[8].connectedSwitch = connectedSwitches[2];
                lights[9].connectedSwitch = connectedSwitches[3];
                break;
            default:
                break;
        }

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool oldOpen = isOpen;
        // NOTE: I've set this up with a predefinte amount of switch amounts due to my plans to add door lights to the door models to indicate how many switches need to be pressed
        switch (numberOfSwitches)
        {
            case 1:
                if (connectedSwitches[0].activated)
                {
                    isOpen = true;
                }
                else isOpen = false;

                break;
            case 2:
                if (connectedSwitches[0].activated && connectedSwitches[1].activated)
                {
                    isOpen = true;
                }
                else isOpen = false;
                break;
            case 3:
                if (connectedSwitches[0].activated && connectedSwitches[1].activated && connectedSwitches[2].activated)
                {
                    isOpen = true;
                }
                else isOpen = false;
                break;
            case 4:
                if (connectedSwitches[0].activated && connectedSwitches[1].activated && connectedSwitches[2].activated && connectedSwitches[3].activated)
                {
                    isOpen = true;
                }
                else isOpen = false;
                break;
            default:
                break;
        }

        if ((oldOpen == false) && (isOpen == true))
        {
            // door just opened, play sound
            SoundFXManager.instance.PlaySpatialSoundFXClip(doorSoundClip, transform, 0.3f);

            if (showObjectiveOnOpen && ObjectiveUI.Instance != null)
            {
                Debug.Log("Door opened: displaying Find your way across");
                ObjectiveUI.Instance.ShowObjective("Find your way across!", 0f);
            }
        }

        anim.SetBool("open", isOpen);

    }
}
