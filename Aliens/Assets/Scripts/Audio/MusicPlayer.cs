public class MusicPlayer : AudioPlayer
{
    private void OnValidate()
    {
        _type = AudioType.Music;
    }

    private new void Start()
    {
        base.Start();

        _type = AudioType.Music;
        Play();
    }
}
