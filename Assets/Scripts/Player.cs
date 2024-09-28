using UnityEngine;
using UnityEngine.Android;

public class Player : MonoBehaviour //Asociado a XR Origin
{
  [SerializeField] float health;
  [SerializeField] Transform head;
  [SerializeField] private BloodEffectPlane bloodEffectPlane;
  public CameraShake cameraShake;

  private bool isConnected;
  public string deviceName = "ESP32_BT";
  public string motor1 = "F";
  public string motor2 = "B";


  bool flag;

  private Evaluator evaluator;

  private void Start() {
#if UNITY_2020_2_OR_NEWER
#if UNITY_ANDROID
    if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
        || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
        || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
        || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
        || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
      Permission.RequestUserPermissions(
        new[] {
          Permission.CoarseLocation,
          Permission.FineLocation,
          "android.permission.BLUETOOTH_SCAN",
          "android.permission.BLUETOOTH_ADVERTISE",
          "android.permission.BLUETOOTH_CONNECT"
        }
      );
#endif
#endif

    isConnected = false;

    BluetoothService.CreateBluetoothObject();

    if (!isConnected) {
      isConnected = BluetoothService.StartBluetoothConnection(deviceName);
    }

    if (!isConnected) {
      isConnected = BluetoothService.StartBluetoothConnection("ESP32_BT");
    }

    BluetoothService.WritetoBluetooth(motor1);
    BluetoothService.WritetoBluetooth("F");


    // Iniciar el temporizador cuando comience la escena

    evaluator = FindObjectOfType<Evaluator>();
  }

  public void TakeDamage(float damage) {
    StartCoroutine(cameraShake.Shake()); //en complete XR Origin


    health -= damage;
    // TODO: Add player take damage 


    Debug.LogWarning($"Player health: {health}");
    //probabilidad 50% de activar 1 motor o el otro
    if (Random.Range(0, 100) < 50) {
      BluetoothService.WritetoBluetooth(motor1);
      BluetoothService.WritetoBluetooth("F");
    } else {
      BluetoothService.WritetoBluetooth(motor2);
      BluetoothService.WritetoBluetooth("B");
    }


    // Verificar si la salud llega a cero o menos
    if (health <= 0) {
      // Detener el temporizador
      // StopTimer();
      if (flag == false) {
        bloodEffectPlane.ShowBloodEffect(); //En XR Origin
        flag = true;
      }

      // ir a la escena de Game Over pero a los 5 segundos
      Invoke(nameof(GameOver), 3f);
    }
  }

  public void SendBtMessage(string message) {
    BluetoothService.WritetoBluetooth(message);
  }

  private void GameOver() {
    evaluator.OnReceiveShot();
  }

  public Vector3 GetHeadPosition() {
    return head.position;
  }
}