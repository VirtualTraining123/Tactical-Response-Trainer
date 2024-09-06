using UnityEngine;

public class CivilFootsteps : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        // Obt�n la instancia del AudioManager
        audioManager = AudioManager.Instance;
    }
    public void PlayFootstepSound()
    {
        audioManager.Play("CivilFootstepSound");
    }
}
