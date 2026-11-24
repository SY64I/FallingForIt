using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for IEnumerator

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject levelsButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject level1Button;
    [SerializeField] private GameObject level2Button;
    [SerializeField] private GameObject level3Button;
    [SerializeField] private GameObject backButton;

    [SerializeField] private AudioClip clickSoundClip;
    [SerializeField] private AudioClip hoverSoundClip;
    [SerializeField] private AudioClip leaveSoundClip;

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
        SceneManager.LoadScene(1);
    }

    public void Levels()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        playButton.SetActive(false);
        levelsButton.SetActive(false);
        quitButton.SetActive(false);
        level1Button.SetActive(true);
        level2Button.SetActive(true);
        level3Button.SetActive(true);
        backButton.SetActive(true);
    }

    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        StartCoroutine(WaitAndQuit());
    }

    public void Level1()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        SceneManager.LoadScene(2);
    }

    public void Level3()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        SceneManager.LoadScene(3);
    }

    public void Back()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        playButton.SetActive(true);
        levelsButton.SetActive(true);
        quitButton.SetActive(true);
        level1Button.SetActive(false);
        level2Button.SetActive(false);
        level3Button.SetActive(false);
        backButton.SetActive(false);
    }

    IEnumerator WaitAndQuit()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
