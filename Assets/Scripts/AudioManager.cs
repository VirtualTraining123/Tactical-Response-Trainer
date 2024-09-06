using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;
    [Range(-3, 3)] public float pitch = 1;
    public bool loop;
    public bool playOnAwake;
    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.playOnAwake = s.playOnAwake;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + clipName + " not found");
            return;
        }

        s.source.Play();
    }

    public void Stop(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);

        if (s != null)
        {
            s.source.Stop();
        }
    }
}