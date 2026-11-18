using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepsSoundClip;
    public void Footstep()
    {
        int random = Random.Range(0, footstepsSoundClip.Length);
        var clip = footstepsSoundClip[random];
        SoundFXManager.instance.PlaySoundFXClip(clip, transform, 1f);
    }
}
