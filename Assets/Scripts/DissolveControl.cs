using UnityEngine;

public class DissolveControl : MonoBehaviour
{
    [SerializeField] PlayerControls playerControl;
    Renderer rend;
    Material mat;

    private static readonly int CutoffHeightID = Shader.PropertyToID("_CutoffHeight");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControl.diedToGoo && !GameManager.instance.respawning)
        {
            rend.material.SetFloat(CutoffHeightID, playerControl.edgeHeightAmount);
        }

        if (GameManager.instance.respawning)
        {
            rend.material.SetFloat(CutoffHeightID, playerControl.edgeHeightMax);
        }
    }
}
