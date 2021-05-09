using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void buttonBeginAction()
    {
        SceneManager.LoadScene("Anchors");
    }

    public void buttonAboutAction()
    {
        SceneManager.LoadScene("About");
    }

    public void buttonBackToMenuAction()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void buttonLeafsAction()
    {
        SceneManager.LoadScene("Folhas");
    }
}
