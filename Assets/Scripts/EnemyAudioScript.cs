using System.Collections;
using UnityEngine;

public class EnemyAudioScript : MonoBehaviour
{
    public string shootAudioName;
    public string deathAudioName;

    private void Start()
    {
        // Puedes reproducir algún sonido al iniciar si lo necesitas
    }

    public void PlayShootSound()
    {
        if (!string.IsNullOrEmpty(shootAudioName))
        {
            AudioManager.instance.Play(shootAudioName);
            //AudioSource.PlayClipAtPoint (shootAudioName, transform.position);
        }
    }

    public void PlayDeathSound()
    {
        if (!string.IsNullOrEmpty(deathAudioName))
        {
            AudioManager.instance.Play(deathAudioName);
        }
    }
}