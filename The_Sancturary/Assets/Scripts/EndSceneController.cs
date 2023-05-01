using UnityEngine;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    public GameObject survivedText;
    public GameObject notSurvivedText;
    public string survivalPlayerPrefKey = "Survived";

    void Start()
    {
        int survived = PlayerPrefs.GetInt(survivalPlayerPrefKey, -1);

        if (survived == 1)
        {
            survivedText.SetActive(true);
            notSurvivedText.SetActive(false);
        }
        else if (survived == 0)
        {
            survivedText.SetActive(false);
            notSurvivedText.SetActive(true);
        }
        else
        {
            Debug.LogError("Player survival status not found.");
        }
    }
}
