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

    public static int shotsFiredPistol = 0;
    
    protected override void StartShooting(XRBaseInteractor interactor)
    {
        base.StartShooting(interactor);
        DrawDebugRaycast();
        Shoot();

    }

    protected override void Shoot()
    {

        base.Shoot();
        
        // Registrar disparo realizado
        shotsFiredPistol++;
        
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
        Vector3 normalizedScale = new Vector3(
            0.4f / parent.localScale.x,
            0.005f / parent.localScale.y,
            0.4f / parent.localScale.z
        );

        // Establecer la escala normalizada para el agujero de bala
        bulletHoleInstance.transform.localScale = normalizedScale;
    }

    private void DrawDebugRaycast()
    {
        // Dibujar el rayo de debug
        Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 100f, Color.yellow, debugRayDuration);
    }


private void OnGUI()
    {
        GUI.Label(new Rect(40, 90, 200, 20), "Disparos realizados: " + shotsFiredPistol);
        
    }

/*
 private void OnDestroy()
    {
        // Almacenar el número de disparos realizados al momento del Game Over o Victoria
        PlayerPrefs.SetInt("PistolShotsFired", shotsFiredPistol);
        PlayerPrefs.Save();
    }
*/
    
}
