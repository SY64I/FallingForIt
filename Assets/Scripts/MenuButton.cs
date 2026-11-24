using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private AudioClip clickSoundClip;
    [SerializeField] private AudioClip hoverSoundClip;
    [SerializeField] private AudioClip leaveSoundClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HoverButton()
    {
        SoundFXManager.instance.PlaySoundFXClip(hoverSoundClip, transform, 1f);
    }
    public void LeaveButton()
    {
        SoundFXManager.instance.PlaySoundFXClip(leaveSoundClip, transform, 1f);
    }
    public void Menu()
    {
        SoundFXManager.instance.PlaySoundFXClip(clickSoundClip, transform, 1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
