using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour
{
    [SerializeField] protected float shootingForce=50;
    [SerializeField] protected Transform bulletSpawn;
    [SerializeField] private float recoilForce;
    [SerializeField] private float damage;

    [SerializeField] private AudioClip[] audios;
    private AudioSource controlAudio;

    private Rigidbody rigidBody;
    private XRGrabInteractable interactableWeapon;
    private AudioManagerEasy audioManagerEasy;
      public InputActionProperty aButtonAction;
    public InputActionProperty bButtonAction;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        SetupInteractableWeaponEvents();
        controlAudio = GetComponent<AudioSource>();

        // Configurar las acciones de los botones A y B
        aButtonAction.action.performed += _ => Reload();
        bButtonAction.action.performed += _ => ToggleSafety();

    }

    [Obsolete("Obsolete")]
    private void SetupInteractableWeaponEvents()
    {
        interactableWeapon.onSelectEntered.AddListener(PickUpWeapon);
        interactableWeapon.onSelectExited.AddListener(DropWeapon);
        interactableWeapon.onActivate.AddListener(StartShooting);
        interactableWeapon.onDeactivate.AddListener(StopShooting);
        //a continuacion se asocia el boton A del controlador con el metodo Reload
        
    }

    protected void SeleccionAudio(int indice, float volumen)
    {
        controlAudio.PlayOneShot(audios[indice], volumen);
    }

  
    private void PickUpWeapon(XRBaseInteractor interactor)
    {
        
        interactor.GetComponent<MeshHidder>().Hide();
        
    }
 
    private void DropWeapon(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHidder>().Show();

    }

    protected virtual void StartShooting(XRBaseInteractor interactor)
    {

    }

    protected virtual void StopShooting(XRBaseInteractor interactor)
    {

    }

    protected virtual void Shoot()
    {
        ApplyRecoil();
        SeleccionAudio(0, 0.2f);
        Invoke("SeleccionAudio(1, 0.2f)", 0.5f); // que usar para reemplazar el invoque pero poder usar el Seleccion audio?
        //Player.SendBTMessage("F");

    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }

    public float GetDamage()
    {
        return damage;
    }


    protected virtual void Reload() { 
        SeleccionAudio(2, 0.2f);
    }

    protected virtual void ToggleSafety() { 
        SeleccionAudio(3, 0.2f);
    }

}

