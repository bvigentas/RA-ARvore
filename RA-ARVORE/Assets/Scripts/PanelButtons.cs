using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtons : MonoBehaviour
{

    GameObject panel;
    Animation animation;
    void Start()
    {
        panel = GameObject.Find("InfoPanel");
        animation = panel.GetComponent<Animation>();
    }

    public void buttonPopupAction()
    {
        //animation.Play("PanelPopUp");
        //GameObject banner = GameObject.FindGameObjectWithTag("BannerMain");
        //if (banner.activeSelf)
        //{
        //    banner.SetActive(false);
        //} else
        //{
        //    banner.SetActive(true);
        //}
        
    }

    public void buttonCloseAction()
    {
        //animation.Play("PanelClose");
    }
}
