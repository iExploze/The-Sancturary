using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class MenuManager : MonoBehaviour
{
    public Canvas jumpscareCanvas;
    public Canvas currentCanvas;
    public GameObject candle;
    public VideoPlayer jumpscareVideo;

    public void PlayGame()
    {
        SceneManager.LoadScene("LoadScene_temp");
    }

    public void QuitGame()
    {
        CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        candle.SetActive(false);
        jumpscareVideo.Play();
        StartCoroutine(WaitForJumpscare());
    }

    public void Options()
    {
        // Add code to handle options.
    }

    public void Credits()
    {
        // Add code to handle credits.
    }

    IEnumerator WaitForJumpscare()
    {
        CanvasGroup canvasGroup = currentCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        // Wait for the length of the jumpscare video
        yield return new WaitForSeconds((float)jumpscareVideo.length);
        Application.Quit();
    }
}
