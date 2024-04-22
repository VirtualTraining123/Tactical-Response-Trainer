using System.Collections;
using UnityEngine;

public class BloodEffectPlane : MonoBehaviour
{
    private Renderer rend;
    private Color startColor;
    private Coroutine currentCoroutine;
    private float duration = 2.5f; // Duración del efecto en segundos

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        rend.material.color = new Color(startColor.r, startColor.g, startColor.b, 0); // Comienza transparente
    }

    public void ShowBloodEffect()
    {
        // Detener el Coroutine anterior si existe
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Iniciar el Coroutine para cambiar la transparencia gradualmente
        currentCoroutine = StartCoroutine(ChangeTransparencyOverTime(startColor.a, 1, duration));
    }

    

    IEnumerator ChangeTransparencyOverTime(float startAlpha, float targetAlpha, float duration)
    {
        float timer = 0f;
        Color currentColor = rend.material.color;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            currentColor.a = alpha;
            rend.material.color = currentColor;
            timer += Time.deltaTime;
            yield return null;
        }

        // Asegurar que el color sea exactamente el objetivo al finalizar
        currentColor.a = targetAlpha;
        rend.material.color = currentColor;
    }
}
