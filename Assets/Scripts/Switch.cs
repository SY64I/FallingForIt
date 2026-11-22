using UnityEngine;
using System;

public class Switch : MonoBehaviour
{
    public bool activated;
    public bool resetUnlessNextCheckpoint;
    bool stayActivated;


    private void Start()
    {
        if (resetUnlessNextCheckpoint)
        {
            GameManager.instance.checkpointHit += SaveActivated;
        }
    }
    private void Update()
    {
        if (resetUnlessNextCheckpoint && !stayActivated && GameManager.instance.respawning)
        {
            Debug.Log("Reseting switch");
            activated = false;
        }
        
    }


    private void SaveActivated() // 
    {
        if (activated)
        {
            stayActivated = true;
        }
    }
}
