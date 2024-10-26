using System;
using UnityEngine;

[Serializable]
public class Sound {
  public string name;
  public AudioClip clip;
  // [Range(0, 1)] public float volume = 1;
  // [Range(-3, 3)] public float pitch = 1;
  // public bool loop;
  // public bool playOnAwake;
  [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour {
  public Sound[] sounds;

  public void Awake() { //aca estaba lo que detonaba pistol?
    foreach (var s in sounds) {
      s.source = gameObject.AddComponent<AudioSource>();
      s.source.bypassEffects = true;
      s.source.bypassListenerEffects = true;
      s.source.bypassReverbZones = true;
      s.source.spatialBlend = 1;
      s.source.rolloffMode = AudioRolloffMode.Linear;
      s.source.maxDistance = 10000;
      s.source.dopplerLevel = 0;
      s.source.spread = 0;
      s.source.clip = s.clip;
      // s.source.playOnAwake = s.playOnAwake;
      // s.source.volume = s.volume;
      // s.source.pitch = s.pitch;
      // s.source.loop = s.loop;
      s.source.clip.LoadAudioData();
    }
  }

  public void Play(string clipName) {
    var s = Array.Find(sounds, sound => string.Equals(sound.name.Trim(), clipName.Trim(), StringComparison.CurrentCultureIgnoreCase));
    Debug.Log($"Playing {clipName}");
    if (s == null) {
      Debug.LogWarning("Sound: " + clipName + " not found");
      return;
    }
    s.source.Play();
    Debug.Log($"Played {clipName}@{s.source.volume} At position {transform.position} load state is {s.source.clip.loadState}");
  }

  public void Stop(string clipName) {
    var s = Array.Find(sounds, sound => sound.name == clipName);

    s?.source.Stop();
  }
}