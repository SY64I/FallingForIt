using UnityEngine;

public class ActivatedMultiLight : MonoBehaviour
{
    // Lights can only be activated by 1 specific switch
    [HideInInspector] public Switch attractSwitch;
    [HideInInspector] public Switch repelSwitch;
    [SerializeField] MeshRenderer lightMesh;
    [SerializeField] Material attractMat, repelMat, offMat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attractSwitch.activated)
        {
            lightMesh.material = attractMat;
        }
        else if (repelSwitch.activated)
        {
            lightMesh.material = repelMat;
        } else 
        {
            lightMesh.material = offMat;
        }
    }
}
