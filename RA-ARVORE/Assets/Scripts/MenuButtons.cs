using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void buttonIniciarAction()
    {
        SceneManager.LoadScene("Anchors");
    }

    public void buttonAboutAction()
    {
        SceneManager.LoadScene("About");
    }

    public void buttonAboutVoltarAction()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
