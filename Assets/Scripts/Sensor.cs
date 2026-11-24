using UnityEngine;

[RequireComponent(typeof(Switch))]
public class Sensor : MonoBehaviour
{
    [SerializeField] Switch switchState;

    [SerializeField] LineRenderer sensorLaser;
    [SerializeField] Transform laserStart, laserEnd;
    [SerializeField] LayerMask layersToDetect;
    [SerializeField] private AudioClip laserSoundClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //switchState = GetComponent<Switch>();
        sensorLaser = GetComponent<LineRenderer>();

        sensorLaser.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!switchState.activated)
        {
            sensorLaser.enabled = true;
            sensorLaser.SetPosition(0, laserStart.position);
            sensorLaser.SetPosition(1, laserEnd.position);

            RaycastHit hit;
            Vector3 direction = (laserEnd.position - laserStart.position).normalized;
            float maxDistance = Vector3.Distance(laserStart.position, laserEnd.position);

            if (Physics.Raycast(laserStart.position, direction, out hit, maxDistance, layersToDetect))
            {
                //Debug.Log("Rayhit");
                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Monster"))
                {
                    //Debug.Log("Activate sensor");
                    // play laser sound effect
                    SoundFXManager.instance.PlaySpatialSoundFXClip(laserSoundClip, transform, 0.3f);
                    switchState.activated = true;
                }
            }
        }
        else
        {
            sensorLaser.enabled = false;
        }
    }
}
