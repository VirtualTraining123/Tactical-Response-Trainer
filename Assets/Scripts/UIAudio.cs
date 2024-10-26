using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
  public string clickAudioName;
  public string hoverEnterAudioName;
  public string hoverExitAudioName;
  private AudioManager audioManager;

  private void Awake() {
    audioManager = FindObjectOfType<AudioManager>();
  }

  public void OnPointerClick(PointerEventData eventData) {
    if (clickAudioName == "") return;
    audioManager.Play(clickAudioName);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    if (hoverEnterAudioName == "") return;
    audioManager.Play(hoverEnterAudioName);
  }

  public void OnPointerExit(PointerEventData eventData) {
    if (hoverExitAudioName == "") return;
    audioManager.Play(hoverExitAudioName);
  }
}