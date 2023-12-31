using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class AudioPlayer : MonoBehaviour
{
    [SerializeField] protected AudioClip _audio;
    [SerializeField] protected AudioType _type;

    private AudioSource _audioSource;

    protected void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    protected void Play()
    {
        if (_audio == null)
            return;

        _audioSource.Stop();
        if (_type == AudioType.Sound)
        {
            _audioSource.loop = false;
            _audioSource.PlayOneShot(_audio, AudioController.Instance.IsMutedSounds ? 0f : 1f);
        }
        else if (_type == AudioType.Music)
        {
            _audioSource.loop = true;
            _audioSource.PlayOneShot(_audio, AudioController.Instance.IsMutedMusic ? 0f : 1f);
        }
    }

    public enum AudioType
    {
        Sound,
        Music
    }
}
