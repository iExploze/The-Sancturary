using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadScene_temp");
    }

    public void QuitGame()
    {
        Debug.Log('L');
        Application.Quit();
    }

    public void Options()
    {
        // Add code to handle options.
    }

    public void Credits()
    {
        // Add code to handle credits.
    }
}
