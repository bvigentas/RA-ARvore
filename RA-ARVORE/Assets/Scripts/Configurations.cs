using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configurations : MonoBehaviour
{
    public static bool quizMode;
    public static GameObject[] hints;

    public void Start()
    {
        DontDestroyOnLoad(this);
        quizMode = false;
    }
}
