using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BlinkEffect))]
public class IntroManager : MonoBehaviour
{

    public static IntroManager Instance;

    void Awake()
    {
        GetComponent<BlinkEffect>().Blink();
    }

}