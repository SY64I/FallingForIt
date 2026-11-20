using UnityEngine;

public class ActivatedLight : MonoBehaviour
{
    // Lights can only be activated by 1 specific switch
    [HideInInspector] public Switch connectedSwitch;
    [SerializeField] MeshRenderer lightMesh;
    [SerializeField] Material lightOffMat, lightOnMat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedSwitch.activated)
        {
            lightMesh.material = lightOnMat;
        }
        else
        {
            lightMesh.material = lightOffMat;
        }
    }
}
