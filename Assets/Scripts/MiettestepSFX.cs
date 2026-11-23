using UnityEngine;

public class MiettestepSFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] miettestepsSoundClip;
    [SerializeField] private AudioClip[] miettekillsSoundClip;
    public void Miettestep()
    {
        int random = Random.Range(0, miettestepsSoundClip.Length);
        var clip = miettestepsSoundClip[random];
        SoundFXManager.instance.PlaySoundFXClip(clip, transform, 1f);
    }
    public void Roar()
    {
        int random = Random.Range(0, miettekillsSoundClip.Length);
        var clip = miettekillsSoundClip[random];
        SoundFXManager.instance.PlaySoundFXClip(clip, transform, 1f);
    }
}