using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{
    public UIController ui;
    public CameraController cam;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cam.enabled = false;
            ui.winPanel.SetActive(true);
        }
    }
}
