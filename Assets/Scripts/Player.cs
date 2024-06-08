using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour //Asociado a XR Origin
{
    [SerializeField] float health;
    [SerializeField] Transform head;
    [SerializeField] private BloodEffectPlane bloodEffectPlane;
    public CameraShake cameraShake;
    private AudioManagerEasy audioManagerEasy;
    public GameObject textoPrefab;

    
    private bool IsConnected;
    public string deviceName= "ESP32_BT";
    public string motor1 = "F";
    public string motor2 = "B";

    private float startTime; // Tiempo de inicio de la escena
    private bool isTimerRunning = false; // Flag para indicar si el temporizador está en funcionamiento

    bool flag=false;

    public static float elapsedTime;

     private Evaluator evaluator;

    private void Start()
    {


       

#if UNITY_2020_2_OR_NEWER
#if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
                || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
                || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
                || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
                || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
                    Permission.RequestUserPermissions(new string[] {
                                Permission.CoarseLocation,
                                    Permission.FineLocation,
                                    "android.permission.BLUETOOTH_SCAN",
                                    "android.permission.BLUETOOTH_ADVERTISE",
                                    "android.permission.BLUETOOTH_CONNECT"
                            });
#endif
#endif

        IsConnected = false;
        
        BluetoothService.CreateBluetoothObject();

        if (!IsConnected)
        {
            IsConnected = BluetoothService.StartBluetoothConnection(deviceName);
            
        }
        if (!IsConnected)
        {
            IsConnected = BluetoothService.StartBluetoothConnection("ESP32_BT");
            
        }
        BluetoothService.WritetoBluetooth(motor1);
        BluetoothService.WritetoBluetooth("F");


        // Iniciar el temporizador cuando comience la escena
        startTime = Time.time;
        isTimerRunning = true;
        evaluator = FindObjectOfType<Evaluator>();
    }
    private void Awake()
    {
        audioManagerEasy = FindObjectOfType<AudioManagerEasy>();
     
    }
/*
    private void Update()
    {
        // Verificar si el temporizador está en funcionamiento
        if (isTimerRunning)
        {
            // Calcular el tiempo transcurrido desde el inicio de la escena
            elapsedTime = Time.time - startTime;

            // Puedes usar "elapsedTime" para mostrar el tiempo transcurrido en algún lugar de la interfaz de usuario
        }
    }

*/
    
/*
    public void StopTimer()
    {
        isTimerRunning = false;
    }
*/
    public void TakeDamage(float damage)
    {

       
        
        StartCoroutine(cameraShake.Shake()); //en complete XR Origin


        
        

        health -= damage;
        audioManagerEasy.SeleccionAudio(0, 3f);
       
        

        Debug.LogWarning(string.Format("Player health: {0}", health));
        //probabilidad 50% de activar 1 motor o el otro
        if( UnityEngine.Random.Range(0, 100) < 50)
        {
            BluetoothService.WritetoBluetooth(motor1);
            BluetoothService.WritetoBluetooth("F");
        }
        else
        {
            BluetoothService.WritetoBluetooth(motor2);
            BluetoothService.WritetoBluetooth("B");
        }

        
        // Verificar si la salud llega a cero o menos
        if (health <= 0)
        {
            // Detener el temporizador
          // StopTimer();
        if(flag==false){
        bloodEffectPlane.ShowBloodEffect(); //En XR Origin
        flag=true;
        }
            // ir a la escena de Game Over pero a los 5 segundos
            Invoke("GameOver", 3f);
          
        }
    }
   
    public void SendBTMessage(string message){
        BluetoothService.WritetoBluetooth(message);
    }

    private void GameOver()
    {

        //PlayerPrefs.SetFloat("Elapsed_Time", elapsedTime);
        

        // Ir a la escena de Game Over
        //SceneManager.LoadScene("3 GameOver");
        evaluator.ReceiveShot();
    }

    public Vector3 GetHeadPosition()
    {
        return head.position;
    }
/*
    private void OnGUI()
    {
      
        GUI.Label(new Rect(450, 20, 300, 20), "Tiempo transcurrido: " + elapsedTime);
    }*/
}
