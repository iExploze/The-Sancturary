using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public Canvas jumpscareCanvas;
    public Canvas currentCanvas;
    public GameObject candle;
    public VideoPlayer jumpscareVideo;


    public void QuitGame()
    {
        CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        candle.SetActive(false);
        jumpscareVideo.Play();
        StartCoroutine(WaitForJumpscare());
    }

    IEnumerator WaitForJumpscare()
    {
        CanvasGroup canvasGroup = currentCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        yield return new WaitForSeconds((float)jumpscareVideo.length);
        Application.Quit();
    }
}
