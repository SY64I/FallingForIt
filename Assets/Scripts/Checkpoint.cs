using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPoint, monsterSpawnPoint;
    public bool oneUseCheckpoint;
    bool previouslyActivated;


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
}
