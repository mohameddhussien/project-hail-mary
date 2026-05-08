using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    public Image blackOverlay;
    public float fadeSpeed = 5f;

    public float blinkDuration = 2f;
    public float blinkInterval = 0.08f;

    public void Blink()
    {
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        float timer = 0f;
        bool isBlack = false;

        while (timer < blinkDuration)
        {
            isBlack = !isBlack;

            Color c = blackOverlay.color;
            c.a = isBlack ? 1f : 0f;
            blackOverlay.color = c;

            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval;
        }

        // Ensure transparent at end
        Color finalColor = blackOverlay.color;
        finalColor.a = 0f;
        blackOverlay.color = finalColor;
    }
}