using Managers;

public interface ISoundSource
{
    AssetsLoader AssetsLoader { get; }
    void Init(AssetsLoader assetsLoader);
    void Play();
    void Stop();
}