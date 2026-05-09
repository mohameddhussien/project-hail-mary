using UnityEngine;

public class IntroVoiceover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        double startTime = AudioSettings.dspTime + 5.0; // Play in exactly 5 seconds
        audioSource.PlayScheduled(startTime);
    }
}
