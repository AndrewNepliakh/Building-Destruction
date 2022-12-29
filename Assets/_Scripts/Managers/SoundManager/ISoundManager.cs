public interface ISoundManager
{
    // void Init();
    void PlayAudioSource<T>() where T : SoundSource;
    void Clear();
}