using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music Source")]
    public AudioSource musicSource;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("End SFX")]
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Volumes")]
    [Range(0f, 1f)] public float menuMusicVolume = 0.6f;
    [Range(0f, 1f)] public float gameplayMusicVolume = 0.6f;
    [Range(0f, 1f)] public float endSfxVolume = 1f;

    void Awake()
    {
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
        }
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic, menuMusicVolume);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic, gameplayMusicVolume);
    }

    public void PlayWin()
    {
        StopMusic();

        if (sfxSource != null && winSound != null)
            sfxSource.PlayOneShot(winSound, endSfxVolume);
    }

    public void PlayLose()
    {
        StopMusic();

        if (sfxSource != null && loseSound != null)
            sfxSource.PlayOneShot(loseSound, endSfxVolume);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    void PlayMusic(AudioClip clip, float volume)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }
}
