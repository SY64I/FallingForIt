using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] private GameObject escapeMenu;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject checkpointButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject menuButton;

    [SerializeField] private AudioClip clickSoundClip;
    [SerializeField] private AudioClip hoverSoundClip;
    [SerializeField] private AudioClip leaveSoundClip;
    public PlayerControls playerControls;

    public void HoverButton()
    {
        SoundFXManager.instance.PlaySoundFXClip(hoverSoundClip, transform, 1f);
    }
    public void LeaveButton()
    {
        SoundFXManager.instance.PlaySoundFXClip(leaveSoundClip, transform, 1f);
    }

    public void PlayGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        escapeMenu.SetActive(false);
    }

    public void Checkpoint()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        escapeMenu.SetActive(false);
        playerControls.Respawn();
    }

    public void RestartGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        escapeMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        escapeMenu.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
