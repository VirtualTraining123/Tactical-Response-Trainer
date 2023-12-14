using UnityEngine;

public class CivilFootsteps : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        // Obtén la instancia del AudioManager
        audioManager = AudioManager.instance;
    }
    public void PlayFootstepSound()
    {
        audioManager.Play("CivilFootstepSound");
    }
}
