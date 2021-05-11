using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TesteSELECT : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField input;

    public void click()
    {
        input.Select();
    }
}
