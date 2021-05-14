using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public void buttonScreenShot()
    {
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        var screenFrame = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenFrame.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenFrame.Apply();

        var timestamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        var fileName = "ARvore-Screenshot-" + timestamp;

        NativeGallery.SaveImageToGallery(screenFrame, "ARvore Screenshots", fileName);
    }
}
