using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class Player : MonoBehaviour
{
  [SerializeField] private float health;
  [SerializeField] private Transform head;
  [SerializeField] private BloodEffectPlane bloodEffectPlane;
  [SerializeField] private Collider bodyCollider;
  public CameraShake cameraShake;

  private const string MOTOR1 = "F";
  private const string MOTOR2 = "B";
  private bool isConnected;
  private bool isShowingBloodEffect;
  private Evaluator evaluator;

  private void Start() {
    RequestBluetoothPermission();
    isConnected = false;
    BluetoothService.CreateBluetoothObject();
    AssertConnected();
    BluetoothService.WritetoBluetooth(MOTOR1);
    evaluator = FindObjectOfType<Evaluator>();
  }

  private static void RequestBluetoothPermission() {
#if (UNITY_ANDROID && UNITY_2020_2_OR_NEWER)
    var permissions = new[] {
      Permission.CoarseLocation,
      Permission.FineLocation,
      "android.permission.BLUETOOTH_SCAN",
      "android.permission.BLUETOOTH_ADVERTISE",
      "android.permission.BLUETOOTH_CONNECT"
    };
    if (permissions.Any(permission => !Permission.HasUserAuthorizedPermission(permission)))
      Permission.RequestUserPermissions(permissions);
#endif
  }

  private void AssertConnected() {
    for (var i = 0; i < 10 && !isConnected; i++)
      isConnected = BluetoothService.StartBluetoothConnection("ESP32_BT");
  }

  public void TakeDamage(float damage) {
    StartCoroutine(cameraShake.Shake());
    health -= damage;
    // TODO: Add player take damage 
    Debug.LogWarning($"Player health: {health}");
    ActivateRandomMotor();
    if (health > 0) return;
    if (isShowingBloodEffect) return;
    bloodEffectPlane.ShowBloodEffect();
    isShowingBloodEffect = true;
    Invoke(nameof(GameOver), 3f);
  }

  private static void ActivateRandomMotor() {
    BluetoothService.WritetoBluetooth(Random.Range(0, 100) < 50 ? MOTOR1 : MOTOR2);
  }

  private void GameOver() {
    evaluator.OnReceiveShot();
  }

  public Vector3 GetHeadPosition() {
    return head.position;
  }
  
  public Vector3 GetBodyCenterPosition() {
    return bodyCollider.bounds.center;
  }

  public bool IsVisibleFrom(Vector3 transformPosition) {
    return Physics.Linecast(transformPosition, GetBodyCenterPosition(), out var hit) &&
           hit.collider == bodyCollider;
  }
}