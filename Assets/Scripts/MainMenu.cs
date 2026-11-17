using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for IEnumerator

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip clickSoundClip;
    [SerializeField] private AudioClip hoverSoundClip;
    [SerializeField] private AudioClip leaveSoundClip;
    public void PlayGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        StartCoroutine(WaitAndQuit());
    }

    public void HoverButton()
    {
        SoundFXManager.instance.PlaySoundFXClip(hoverSoundClip, transform, 1f);
    }
    public void LeaveButton()
    {
        SoundFXManager.instance.PlaySoundFXClip(leaveSoundClip, transform, 1f);
    }

    IEnumerator WaitAndQuit()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
