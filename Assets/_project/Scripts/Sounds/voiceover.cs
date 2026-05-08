using UnityEngine;

public class voiceover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        double startTime = AudioSettings.dspTime + 10.0; // Play in exactly 10 seconds
        audioSource.PlayScheduled(startTime);
    }
}
