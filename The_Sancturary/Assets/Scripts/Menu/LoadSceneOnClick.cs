using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public string sceneName;

    public void LoadSceneByName()
    {
        SceneManager.LoadScene(sceneName);
    }
}
