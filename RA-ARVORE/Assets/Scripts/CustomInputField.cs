using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomInputField : MonoBehaviour
{
    [SerializeField]
    public TextMeshPro text;

    public void click()
    {
        if (Configurations.quizMode) 
        {
            var keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable, false, false, false, false);
            StartCoroutine(EditTextWithKeyboard(keyboard, text));
        }
    }

    IEnumerator EditTextWithKeyboard(TouchScreenKeyboard keyboard, TextMeshPro t)
    {
        while (!keyboard.done)
        {
            t.text = keyboard.text;
            yield return null;
        }
    }
}
