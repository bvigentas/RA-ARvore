using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Quiz : MonoBehaviour
{
    public void buttonValidar()
    {
        GameObject[] hints = GameObject.FindGameObjectsWithTag("LeafHint");
        GameObject[] inputs = GameObject.FindGameObjectsWithTag("LeafInput");
        List<string> erros = new List<string>();

        for (var i = 0; i < hints.Length; i++)
        {
            var hint = hints[i].GetComponent<TextMeshPro>();
            var input = inputs[i].GetComponent<TMP_InputField>();

            if (!string.Equals(hint.text, input.text, StringComparison.InvariantCultureIgnoreCase))
            {
                erros.Add("Você colocou '" + input.text + "' mas o correto é '" + hint.text + "'.\n");
            }
        }

        if (erros.Count > 0)
        {
            var canvasGroup = GameObject.Find("Red").GetComponent<CanvasGroup>();
            var textoErros = GameObject.Find("Text_Erros").GetComponent<Text>();
            textoErros.text = "";
            foreach (string erro in erros)
            {
                textoErros.text += erro;
            }
            StartCoroutine(ShowCanvas(canvasGroup));
        }
        else
        {
            var canvasGroup = GameObject.Find("Green").GetComponent<CanvasGroup>();
            StartCoroutine(ShowCanvas(canvasGroup));

        }
    }

    IEnumerator ShowCanvas(CanvasGroup canvas)
    {
        while (canvas.alpha < 1)
        {
            canvas.alpha = canvas.alpha + 0.1f;
            yield return null;
        }

        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        yield return new WaitForSeconds(4);

        while (canvas.alpha > 0)
        {
            canvas.alpha = canvas.alpha - 0.1f;
            yield return null;
        }

        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        yield return null;
    }
}
