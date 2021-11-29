using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MiscSoundsManager : MonoBehaviour
{
    public static MiscSoundsManager Instance;

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
    }

    public void PlayClip(AudioClip clip)
    {
        if (source.isPlaying) return;
        source.clip = clip;
        source.Play();
    }
}
