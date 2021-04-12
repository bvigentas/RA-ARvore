using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public void buttonScreenShot()
    {
        var timestamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        string fileName = "ARvore-Screenshot-" + timestamp;
    }
}
