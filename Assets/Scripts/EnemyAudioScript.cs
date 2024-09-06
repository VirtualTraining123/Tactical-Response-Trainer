using UnityEngine;

public class EnemyAudioScript : MonoBehaviour
{
    public string shootAudioName;
    public string deathAudioName;

    public void PlayShootSound()
    {
        if (!string.IsNullOrEmpty(shootAudioName))
        {
            AudioManager.Instance.Play(shootAudioName);
            //AudioSource.PlayClipAtPoint (shootAudioName, transform.position);
        }
    }
//posible uso futuro
    public void PlayDeathSound()
    {
        if (!string.IsNullOrEmpty(deathAudioName))
        {
            AudioManager.Instance.Play(deathAudioName);
        }
    }
}