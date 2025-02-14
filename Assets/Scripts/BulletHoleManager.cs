using UnityEngine;

public class BulletHoleManager : MonoBehaviour {
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private float decalOffset = 0.005f; // Add this field
    private static BulletHoleManager instance;

    public static BulletHoleManager Instance {
        get => instance;
        private set => instance = value;
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void CreateBulletHole(Vector3 position, Vector3 normal, Transform parent) {
        if (!ShouldCreateBulletHole(parent)) return;

        // Align the bullet hole's forward direction with the normal
        Quaternion rotation = Quaternion.LookRotation(normal);
        
        // Offset slightly along the normal to prevent z-fighting
        Vector3 offsetPosition = position + (normal * decalOffset);
        GameObject bulletHoleInstance = Instantiate(bulletHolePrefab, offsetPosition, rotation);

        bulletHoleInstance.transform.SetParent(parent, true);
    }

    private bool ShouldCreateBulletHole(Transform hitTransform) {
        if (hitTransform == null) return false;
        
        var collider = hitTransform.GetComponent<Collider>();
        if (collider == null) return true;

        return !collider.CompareTag("Enemy") && 
               !collider.CompareTag("Civil") && 
               !collider.CompareTag("Bullet") && 
               !collider.CompareTag("Weapon");
    }
}