using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private GameObject screenFlash;

    public IEnumerator Shake(float shakeDuration, float shakeMagnitude, float flashDuration)
    {
        Debug.Log(transform.localPosition);
        Vector3 originalPos = transform.localPosition;

        float elapsedShake = 0.0f;
        float elapsedFlash = 0.0f;

        while (elapsedShake < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * shakeMagnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            Debug.Log(transform.localPosition);

            elapsedShake += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;

        while (elapsedFlash < flashDuration)
        {
            float scale = Mathf.Lerp(1f, 0f, elapsedFlash / flashDuration);
            screenFlash.transform.localScale = new Vector3(scale, scale, scale);

            elapsedFlash += Time.deltaTime;
            yield return null;
        }

        // Ensure the screenFlash is fully faded out
        screenFlash.transform.localScale = new Vector3(0, 0, 0);
    }
}
