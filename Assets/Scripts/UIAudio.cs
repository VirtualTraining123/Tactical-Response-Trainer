using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
  public string clickAudioName;
  public string hoverEnterAudioName;
  public string hoverExitAudioName;
  private AudioManager audioManager;

  private void Awake() {
    audioManager = FindObjectOfType<AudioManager>();
    audioManager.Request(clickAudioName, gameObject);
    audioManager.Request(hoverEnterAudioName, gameObject);
    audioManager.Request(hoverExitAudioName, gameObject);
  }

  public void OnPointerClick(PointerEventData eventData) {
    if (clickAudioName == "") return;
    audioManager.Play(clickAudioName, gameObject);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    if (hoverEnterAudioName == "") return;
    audioManager.Play(hoverEnterAudioName, gameObject);
  }

  public void OnPointerExit(PointerEventData eventData) {
    if (hoverExitAudioName == "") return;
    audioManager.Play(hoverExitAudioName, gameObject);
  }
}