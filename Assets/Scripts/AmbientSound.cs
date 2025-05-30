using UnityEngine;

public class AmbientSound : MonoBehaviour {
  protected AudioManager audioManager;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Awake() {
    audioManager = FindFirstObjectByType<AudioManager>();
    audioManager.Request("Ambient", gameObject);
    audioManager.Play("Ambient", gameObject);
  }

  // Update is called once per frame
  void Update() { }
}
