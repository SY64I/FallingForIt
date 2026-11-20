using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] Switch connectedSwitch;
    [SerializeField] ActivatedLight magnetLight;

    // Temporary bool for magnet being on and off
    bool magnetOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magnetLight.connectedSwitch = connectedSwitch;
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedSwitch.activated)
        {
            magnetOn = true;
        }
        else
        {
            magnetOn = false;
        }
    }
}
