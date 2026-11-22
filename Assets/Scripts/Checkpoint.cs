using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPoint, monsterSpawnPoint;
    public bool oneUseCheckpoint;
    bool previouslyActivated;

    [SerializeField] Color editorCheckpointColour;
    BoxCollider col;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if ((oneUseCheckpoint && !previouslyActivated) || !oneUseCheckpoint)
            {
                Debug.Log("Checkpoint!");
                GameManager.instance.SetSpawnPoints(playerSpawnPoint, monsterSpawnPoint);
                if (oneUseCheckpoint)
                {
                    previouslyActivated = true;
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = editorCheckpointColour;
        Gizmos.matrix = transform.localToWorldMatrix;
        col = GetComponent<BoxCollider>();

        Gizmos.DrawCube(col.center, col.size);

    }
}
