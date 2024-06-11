using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Pistol : Weapon
{
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private float debugRayDuration = 2f;
    public GameObject bulletHolePrefab;

    public static int shotsFiredPistol = 0;

    private Evaluator evaluator;
    private int maxBullets = 12;
    private int currentBullets;
    public bool isSafetyOn = false;
   // public bool isMagazineLoaded = false;

    //public InputActionProperty aButtonAction;
    //public InputActionProperty bButtonAction;

    protected void Start()
    {
        
        currentBullets = 12;
        evaluator = FindObjectOfType<Evaluator>();


        // Registrar callbacks
        //aButtonAction.action.performed += context => OnSelectAction();
        //bButtonAction.action.performed += context => OnActivateAction();

        // Activar las acciones
        aButtonAction.action.Enable();
        bButtonAction.action.Enable();
    }

    
    protected override void StartShooting(XRBaseInteractor interactor)
    {
        if (isSafetyOn || currentBullets <= 0){
            SeleccionAudio(4, 1f);
            return;
        }else {
        base.StartShooting(interactor);
        DrawDebugRaycast();
        Shoot();
        }
    }

    protected override void Shoot()
    {
        
        if (isSafetyOn || currentBullets <= 0){
            SeleccionAudio(4, 1f);
            
        }else{
           
        base.Shoot();
        shotsFiredPistol++;
        currentBullets--;
        

        Projectile projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        projectileInstance.Init(this);
        projectileInstance.Launch();

        

        RaycastHit hit;
        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit))
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Civil") || hit.collider.CompareTag("Bullet"))
            {
                return;
            }
            InstantiateBulletHole(hit.point, hit.normal, hit.collider.transform);
        }
        evaluator.BulletUsed();
    }
    }

    private void InstantiateBulletHole(Vector3 position, Vector3 normal, Transform parent)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        
        GameObject bulletHoleInstance = Instantiate(bulletHolePrefab, position, rotation);
        bulletHoleInstance.transform.localScale = new Vector3(0.2f, 0.005f, 0.2f);
        bulletHoleInstance.transform.SetParent(parent, true);
        
    }

    private void DrawDebugRaycast()
    {
        Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 100f, Color.yellow, debugRayDuration);
    }

    private void OnSelectAction()
    {
        ToggleSafety();
    }

    private void OnActivateAction()
    {
        Reload();
    }
    protected override void Reload()
    {
        base.Reload();
        
        currentBullets = maxBullets;
       
            
       
    }

    public void callReload()
    {
        Reload();
    }

    public int GetBullets()
    {
        return currentBullets;
    }

    protected override void ToggleSafety()
    {
      base.ToggleSafety();

        isSafetyOn = !isSafetyOn;
    }

    public void callToggleSafety()
    {
        ToggleSafety();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(40, 90, 200, 20), "Disparos realizados: " + shotsFiredPistol);
    }
}
