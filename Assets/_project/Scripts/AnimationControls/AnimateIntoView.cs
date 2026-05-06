using UnityEngine;
using System.Collections;

public class AnimateTransform : MonoBehaviour
{
    [Header("Start Transform")]
    public Vector3 startPosition;
    public Vector3 startEulerRotation;
    public Vector3 startScale = Vector3.one;

    [Header("Target Transform")]
    public Vector3 targetPosition = new(148.6000061f, 859.5200195f, -644.8099976f);
    public Vector3 targetEulerRotation = new(0f, 0f, 0f);
    public Vector3 targetScale = Vector3.one;

    [Header("Animation Settings")]
    public float duration = 1.2f;
    public float delay = 0f;
    public bool playOnStart = true;

    private Coroutine animRoutine;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        // Apply start transform immediately
        transform.SetPositionAndRotation(startPosition, Quaternion.Euler(startEulerRotation));
        transform.localScale = startScale;

        animRoutine = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float time = 0f;

        Quaternion startRot = Quaternion.Euler(startEulerRotation);
        Quaternion targetRot = Quaternion.Euler(targetEulerRotation);

        while (time < duration)
        {
            float t = time / duration;

            // 🎯 Smooth ease-out
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, easedT), Quaternion.Slerp(startRot, targetRot, easedT));
            transform.localScale = Vector3.Lerp(startScale, targetScale, easedT);

            time += Time.deltaTime;
            yield return null;
        }

        // Snap to exact final values
        transform.SetPositionAndRotation(targetPosition, targetRot);
        transform.localScale = targetScale;
    }
}