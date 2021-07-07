using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var hints = GameObject.FindGameObjectsWithTag("LeafHint").OrderBy(obj => obj.name).ToArray();
        var inputs = GameObject.FindGameObjectsWithTag("LeafInput");

        Configurations.hints = hints;

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
