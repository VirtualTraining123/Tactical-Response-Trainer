using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class Pistol : Weapon
{
    [SerializeField] private Projectile bulletPrefab;
    //////////////////////////
    [SerializeField] private float debugRayDuration = 2f;
    public GameObject bulletHolePrefab;
    
    protected override void StartShooting(XRBaseInteractor interactor)
    {
        base.StartShooting(interactor);
        DrawDebugRaycast();
        Shoot();

    }

    protected override void Shoot()
    {

        base.Shoot();
        
        
        Projectile projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        projectileInstance.Init(this);
        projectileInstance.Launch();

         RaycastHit hit;
        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit))
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Civil"))
            {
            // No hacer nada si el objeto impactado es un enemigo o un civil
            return; 
            }
            // Instanciar el objeto bullet hole en el punto de impacto
            InstantiateBulletHole(hit.point, hit.normal, hit.collider.transform);
        }


       
         
       

    }

    protected override void StopShooting(XRBaseInteractor interactor)
    {
        base.StopShooting(interactor);
    }

/////////////////////////////////////////////////////////
///
     private void InstantiateBulletHole(Vector3 position, Vector3 normal, Transform parent)
    {
        // Calcular la rotación necesaria para alinear el eje hacia arriba con la normal de la superficie
    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

    // Instanciar el objeto bullet hole en el punto de impacto con la rotación adecuada y como hijo del objeto impactado
    GameObject bulletHoleInstance = Instantiate(bulletHolePrefab, position, rotation, parent);
    }

    private void DrawDebugRaycast()
    {
        // Dibujar el rayo de debug
        Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 100f, Color.yellow, debugRayDuration);
    }

    
}
