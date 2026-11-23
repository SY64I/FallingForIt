using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private bool transitionStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerControls>() == null)
            return;

        if (transitionStarted)
            return;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("Next Scene Name has not been assigned!");
            return;
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance not found!");
            return;
        }

        transitionStarted = true;

        Debug.Log("Loading next level: " + nextSceneName);

        GameManager.instance.StartCoroutine(GameManager.instance.TransitionLevel(nextSceneName));
    }
}
