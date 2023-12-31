using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public bool IsMutedMusic { get => _isMutedMusic; private set => _isMutedMusic = value; }
    public bool IsMutedSounds { get => _isMutedSounds; private set => _isMutedSounds = value; }

    [SerializeField] private bool _isMutedMusic;
    [SerializeField] private bool _isMutedSounds;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        // Load mute
    }
}
