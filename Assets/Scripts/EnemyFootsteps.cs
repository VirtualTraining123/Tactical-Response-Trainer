using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    public void PlayFootstepSound() //lo borramos? xd
    {
        AudioManager.Instance.Play("EnemyFootstepSound");
    }
}

