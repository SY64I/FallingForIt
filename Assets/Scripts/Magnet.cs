using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] Switch connectedSwitch;
    [SerializeField] ActivatedLight magnetLight;
    [SerializeField] private AudioClip magnetSoundClip;

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
        bool magnetOld = magnetOn;

        if (connectedSwitch.activated)
        {
            magnetOn = true;
        }
        else
        {
            magnetOn = false;
        }

        if ((magnetOld == false) && (magnetOn == true))
        {
            // magnet was just activated, play sound
            SoundFXManager.instance.PlaySoundFXClip(magnetSoundClip, transform, 0.5f);
        }
    }
}
