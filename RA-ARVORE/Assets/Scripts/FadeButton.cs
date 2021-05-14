using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FadeButton : MonoBehaviour
{
    [SerializeField]
    public List<string> leafs;

    [SerializeField]
    public List<RawImage> images;

    public void Start()
    {
        images.ForEach(image => image.enabled = false);
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(false, ""));
    }

    public void FadeIn(string leaf)
    {
        StartCoroutine(Fade(true, leaf));
    }

    IEnumerator Fade(bool fadeIn, string leaf)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (leaf != "")
        {
            var textComponent = GameObject.Find("TextLeaf").GetComponent<Text>();
            textComponent.text = leaf;
            var index = leafs.IndexOf(leaf);
            var image = images.ElementAt(index);
            image.enabled = true;
        }
        
        if (fadeIn)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha = canvasGroup.alpha + 0.1f;
                yield return null;
            }

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        } else
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = canvasGroup.alpha - 0.1f;
                yield return null;
            }

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            images.ForEach(image => image.enabled = false);
        }
        
        yield return null;
    }
}
