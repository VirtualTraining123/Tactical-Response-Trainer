using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Weapon Weapon;
    
    public virtual void Init(Weapon weapon)
    {
        this.Weapon = weapon;
    }

    public virtual void Launch()
    {

    }
}
