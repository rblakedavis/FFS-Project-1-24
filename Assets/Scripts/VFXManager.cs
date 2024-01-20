using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VFXManager : MonoBehaviour
{
    [SerializeField] Image[] healthBar;
    [SerializeField] Image[] magicBar;
    [SerializeField] Image[] shieldBar;

    private Coroutine healthDownCoroutine;
    private Coroutine shieldDownCoroutine;
    private Coroutine magicDownCoroutine;
    private Coroutine shieldBreakCoroutine;

    #region public methods
    public void HealthDown()
    {
        healthDownCoroutine = StartCoroutine(HealthDownCoroutine());
    }

    public void ShieldDown()
    {
        shieldDownCoroutine = StartCoroutine(ShieldDownCoroutine());
    }

    public void MagicDown()
    {
        magicDownCoroutine = StartCoroutine(MagicDownCoroutine());
    }

    public void ShieldBreak()
    {
        shieldBreakCoroutine = StartCoroutine(ShieldBreakCoroutine());
    }
    #endregion

    #region coroutines
    private IEnumerator HealthDownCoroutine()
    {
        yield return null;
    }


    private IEnumerator ShieldDownCoroutine(float duration = 0.2f, float hitFlashDuration = 0.2f)
    {
        // Store the original scale of all shield bars
        Vector3[] originalScales = new Vector3[shieldBar.Length];
        for (int i = 0; i < shieldBar.Length; i++)
        {
            originalScales[i] = shieldBar[i].transform.localScale;
        }

        // Apply a growing animation to all shield bars
        float growScale = 1.2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolate the scale over time
            float scaleRatio = Mathf.Lerp(1f, growScale, elapsedTime / duration);

            // Apply the scaled size to all shield bars
            for (int i = 0; i < shieldBar.Length; i++)
            {
                shieldBar[i].transform.localScale = originalScales[i] * scaleRatio;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all shield bars are at their maximum size
        for (int i = 0; i < shieldBar.Length; i++)
        {
            shieldBar[i].transform.localScale = originalScales[i] * growScale;
        }

        // Wait for the specified hitFlashDuration
        yield return new WaitForSeconds(hitFlashDuration);

        // Reset the scale of all shield bars to the original size gradually
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolate the scale over time
            float scaleRatio = Mathf.Lerp(growScale, 1f, elapsedTime / duration);

            // Apply the scaled size to all shield bars
            for (int i = 0; i < shieldBar.Length; i++)
            {
                shieldBar[i].transform.localScale = originalScales[i] * scaleRatio;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure all shield bars are back to their original size
        for (int i = 0; i < shieldBar.Length; i++)
        {
            shieldBar[i].transform.localScale = originalScales[i];
        }
    }

    private IEnumerator MagicDownCoroutine()
    {
        yield return null;
    }

    private IEnumerator ShieldBreakCoroutine()
    {
        yield return null;
    }
    #endregion
}
