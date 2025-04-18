using UnityEngine;

public class GateOpener : MonoBehaviour {
    [Header("Movimiento")]
    [SerializeField] private float moveDistance = 5f;       // Distancia a mover en el eje X
    [SerializeField] private float moveDuration = 10f;       // Tiempo que tarda en abrirse

    [Header("Inicio automático")]
    [SerializeField] private bool openOnStart = true;       // Abrir automáticamente al iniciar

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float timer;
    private bool isOpening = false;

    private void Start() {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(moveDistance, 0, 0);

        if (openOnStart) {
            StartOpening();
        }
    }

    public void StartOpening() {
        if (!isOpening) {
            isOpening = true;
            timer = 0f;
        }
    }

    private void Update() {
        if (isOpening) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / moveDuration);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, progress);

            if (progress >= 1f) {
                isOpening = false; // Detiene el movimiento
            }
        }
    }
}