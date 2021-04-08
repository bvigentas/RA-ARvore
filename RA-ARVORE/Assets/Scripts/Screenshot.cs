using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{

    [SerializeField]
    GameObject blink;
    public void buttonScreenShot()
    {
        StartCoroutine("CaptureScreen");
    }

    IEnumerator CaptureScreen()
    {
        var timestamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        string fileName = "ARvore" + timestamp + ".png";
        string savePath = fileName;
        ScreenCapture.CaptureScreenshot(savePath);
        yield return new WaitForEndOfFrame();
        Instantiate(blink, new Vector2(0f, 0f), Quaternion.identity);
    }
}
