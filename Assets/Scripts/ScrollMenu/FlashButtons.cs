using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashButtons : MonoBehaviour
{
    public Scrollbar scrollbar;
    public Button upButton;
    public Button downButton;

    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;
    public int framesBetweenFlashes = 3;

    private Color originalUpButtonColor;
    private Color originalDownButtonColor;

    private void Start()
    {
        // Save the original colors of the buttons
        originalUpButtonColor = upButton.image.color;
        originalDownButtonColor = downButton.image.color;

        // Start the flash coroutine
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        while (true)
        {
            // Check the scroll bar value and update button colors accordingly
            if (scrollbar.value <= 0.95f)
            {
                FlashButton(upButton);
            }

            if (scrollbar.value >= 0.05f)
            {
                FlashButton(downButton);
            }

            // Wait for a few frames before the next flash
            yield return new WaitForSecondsRealtime(framesBetweenFlashes * Time.fixedDeltaTime);
        }
    }

    private void FlashButton(Button button)
    {
        // Flash the button by changing its color temporarily
        button.image.color = flashColor;

        // Reset the color after the flash duration
        StartCoroutine(ResetColorAfterDelay(button, flashDuration));
    }

    private IEnumerator ResetColorAfterDelay(Button button, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // Reset the color
        button.image.color = button == upButton ? originalUpButtonColor : originalDownButtonColor;
    }
}
