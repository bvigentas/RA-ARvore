using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] hints = GameObject.FindGameObjectsWithTag("LeafHint");
        GameObject[] inputs = GameObject.FindGameObjectsWithTag("LeafInput");

        if (Configurations.quizMode)
        {
            for (var i = 0; i < inputs.Length; i ++)
            {
                inputs[i].SetActive(true);
                hints[i].SetActive(false);
            }
        } else
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                inputs[i].SetActive(false);
                hints[i].SetActive(true);
            }
        }
    }
}
