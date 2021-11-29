using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Range(0, 0.4f)]
    [SerializeField] float maxVolume;
    [Range(0, 5)]
    [SerializeField] float fadeDuration;
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip[] inGameMusic;

    public static MusicManager Instance;
    int currentInGameSongIndex;
    bool inGame = false;
    bool fading = false;

    AudioSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        source = GetComponent<AudioSource>();
        source.volume = maxVolume;

        currentInGameSongIndex = 0;

        playClip(mainMenuMusic);
    }

    private void Update()
    {
        if (inGame && !fading)
        {
            float timeRemaining = source.clip.length - source.time;

            if (timeRemaining <= fadeDuration)
            {
                currentInGameSongIndex++;
                if (currentInGameSongIndex >= inGameMusic.Length) currentInGameSongIndex = 0;
                fadeToNewSong(inGameMusic[currentInGameSongIndex], fadeDuration);
            }
        }
    }

    public void PlayInGameMusic()
    {
        inGame = true;
        fadeToNewSong(inGameMusic[currentInGameSongIndex], fadeDuration);
    }

    void fadeToNewSong(AudioClip newSong, float fadeDuration)
    {
        StartCoroutine(coFadeBetweenSongs(newSong, fadeDuration));
    }

    IEnumerator coFadeBetweenSongs(AudioClip newSong, float fadeDuration)
    {
        fading = true;
        float fadeTime = 0;
        float initalVolume = source.volume;

        while (fadeTime < fadeDuration)
        {
            source.volume = Mathf.Lerp(initalVolume, 0, fadeTime / fadeDuration);
            yield return null;
            fadeTime += Time.deltaTime;
        }

        source.clip = newSong;
        source.Play();
        fadeTime = 0;

        while (fadeTime < fadeDuration)
        {
            source.volume = Mathf.Lerp(0, initalVolume, fadeTime / fadeDuration);
            yield return null;
            fadeTime += Time.deltaTime;
        }

        fading = false;
    }

    void playClip(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}
