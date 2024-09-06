using UnityEngine;
using UnityEngine.UI;

public class SetOptionFromUI : MonoBehaviour
{
    public Scrollbar volumeSlider;
    public TMPro.TMP_Dropdown turnDropdown;
    public SetTurnTypeFromPlayerPref turnTypeFromPlayerPref;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
        turnDropdown.onValueChanged.AddListener(SetTurnPlayerPref);

        if (PlayerPrefs.HasKey("turn"))
            turnDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("turn"));
    }

    private void SetGlobalVolume(float value)
    {
        AudioListener.volume = value;
    }

    private void SetTurnPlayerPref(int value)
    {
        PlayerPrefs.SetInt("turn", value); 
        turnTypeFromPlayerPref.ApplyPlayerPref();
    }
}
