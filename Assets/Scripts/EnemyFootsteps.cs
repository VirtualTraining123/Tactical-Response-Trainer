using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        // Obtén la instancia del AudioManager
        audioManager = AudioManager.instance;
    }
    public void PlayFootstepSound()
    {
        AudioManager.instance.Play("EnemyFootstepSound");
    }
}

