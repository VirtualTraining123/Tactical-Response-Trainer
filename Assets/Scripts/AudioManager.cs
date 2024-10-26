using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound {
  public string name;
  public AudioClip clip;
}

public class AudioManager : MonoBehaviour {
  public Sound[] sounds;
  private Dictionary<Tuple<string, GameObject>, AudioSource> audioSources = new();

  public void Play(string clipName, GameObject obj) {
    var s = audioSources[new Tuple<string, GameObject>(clipName, obj)];
    if (s == null) {
      Debug.LogWarning("Sound: " + clipName + " not found");
      return;
    }
    s.Play();
    Debug.Log($"Played {clipName}@{s.volume} At position {transform.position} load state is {s.clip.loadState}");
  }

  public void Request(string clipName, GameObject obj) {
    if (audioSources.ContainsKey(new Tuple<string, GameObject>(clipName, obj))) return;
    var s = Array.Find(sounds, sound => sound.name == clipName);
    if (s == null) {
      Debug.LogWarning("Sound: " + clipName + " not found");
      return;
    }
    var audioSource = obj.AddComponent<AudioSource>();
    audioSource.clip = s.clip;
    audioSource.bypassEffects = true;
    audioSource.bypassListenerEffects = true;
    audioSource.bypassReverbZones = true;
    audioSource.spatialBlend = 1;
    audioSource.rolloffMode = AudioRolloffMode.Linear;
    audioSource.maxDistance = 10000;
    audioSource.dopplerLevel = 0;
    audioSource.spread = 0;
    audioSource.clip = s.clip;
    audioSource.clip.LoadAudioData();
    audioSources.Add(new Tuple<string, GameObject>(clipName, obj), audioSource);
  }
}