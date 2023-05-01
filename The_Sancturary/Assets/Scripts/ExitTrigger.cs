using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public string nextScene;
    public string survivalPlayerPrefKey = "Survived";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerSurvived())
            {
                PlayerPrefs.SetInt(survivalPlayerPrefKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(survivalPlayerPrefKey, 0);
            }
            PlayerPrefs.Save();

            SceneManager.LoadScene(nextScene);
        }
    }

    private bool PlayerSurvived()
    {
        // Check for player survival condition.
        // For example, you could check the player's health or other factors.
        // Replace this with your own logic.
        return true;
    }
}
