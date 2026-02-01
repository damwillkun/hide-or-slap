using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float Duration;
    public float Strength;

    Vector3 initialPosition;
    Coroutine shakeCoroutine;

    void Awake()
    {
        initialPosition = transform.localPosition;
    }

    public void Shake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float t = 0f;

        while (t < Duration)
        {
            Vector3 offset = Random.insideUnitSphere * Strength;
            transform.localPosition = initialPosition + offset;

            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;
        shakeCoroutine = null;
    }
}