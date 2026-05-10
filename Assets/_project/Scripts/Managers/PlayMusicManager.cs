
using System;
using UnityEngine;

public class PlayMusicManager : MonoBehaviour
{

    void OnEnable()
    {
        MusicManager.Instance.PlayPatrolMusic();
    }
}
