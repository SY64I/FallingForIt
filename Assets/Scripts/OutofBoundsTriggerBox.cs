using UnityEngine;

public class OutofBoundsTriggerBox : MonoBehaviour
{
    [SerializeField] Color editorKillBoxColour;
    BoxCollider col;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControls controls = other.GetComponent<PlayerControls>();

            if (!controls.GodMode)
            {
                controls.DeathByFalling();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = editorKillBoxColour;
        Gizmos.matrix = transform.localToWorldMatrix;
        col = GetComponent<BoxCollider>();

        Gizmos.DrawCube(col.center, col.size);

    }
}
