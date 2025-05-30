using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Audio {
  public class MissingAudioSourceException : Exception {
    public MissingAudioSourceException(string message) : base(message) { }
  }
  
  public class StringValue : Attribute {
    public string Value { get; }
    public StringValue(string value) {
      Value = value;
    }
  }
  public enum ClipName {
    [StringValue("PavementTiles_Mono_03")]
    FOOTSTEPS,
    [StringValue("Weapons_SMG_Shoot")]
    GUNSHOT,
    [StringValue("bulletshells02")]
    BULLET_SHELLS,
    [StringValue("Dry-Pistol-Gunfire-Single-A")]
    DRY_FIRE,
    [StringValue("Weapon_Safety")]
    SAFETY_TOGGLE,
    [StringValue("reloadSound")]
    RELOAD
  }
  public static class ClipNameMethods {
    public static string GetString(this ClipName clipName) {
      var type = typeof(ClipName);
      var memberInfo = type.GetMember(clipName.ToString());
      if (memberInfo.Length <= 0) return clipName.ToString();
      var attributes = memberInfo[0].GetCustomAttributes(typeof(StringValue), false);
      return attributes.Length > 0 ? ((StringValue)attributes[0]).Value : clipName.ToString();
    }
    public static ClipName GetClipName(this string value) {
      foreach (ClipName clipName in Enum.GetValues(typeof(ClipName))) {
        if (clipName.GetString().Equals(value, StringComparison.OrdinalIgnoreCase)) {
          return clipName;
        }
      }
      throw new ArgumentException($"No ClipName found for value: {value}");
    }
  }
  public class InlineAudioManager {
    private readonly Dictionary<ClipName, AudioSource> audioSources = new();
    
    public void Scan(Component gameObject){
      foreach (var audioSource in gameObject.GetComponentsInChildren<AudioSource>()) {
        if (!audioSource.clip) continue;
        var clipName = audioSource.clip.name.GetClipName();
        if (audioSources.TryAdd(clipName, audioSource)) {
          Debug.Log($"Registered audio source for {clipName} at {gameObject.name}");
        } else {
          Debug.LogWarning($"Audio source for {clipName} already exists at {gameObject.name}");
        }
      }
    }

    public void RequestAudioSources(params ClipName[] clipNames) {
      // Assert contains all clip names
      var missingClips = clipNames.Where(clipName => !audioSources.ContainsKey(clipName));
      var missingClipsArray = missingClips as ClipName[] ?? missingClips.ToArray();
      if (!missingClipsArray.Any()) return;
      throw new MissingAudioSourceException($"Missing audio sources: {string.Join(", ", missingClipsArray)}");
    }
    
    public AudioSource GetAudioSource(ClipName clipName) {
      return audioSources.GetValueOrDefault(clipName);
    }
  }
}
