using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GeoMonsterAI : MonsterBaseAI
{
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public string deathSceneName = "Lobby Scene";

    protected override void PerformAttack()
    {
        agent.isStopped = true;

        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 1;
            jumpscareVideo.Play();
        }

        animator?.SetFloat("Speed", 0f);

        StartCoroutine(WaitForJumpscare());
    }

    private System.Collections.IEnumerator WaitForJumpscare()
    {
        yield return new WaitForSeconds((float)jumpscareVideo.length - 0.3f);

        PlayerPrefs.SetInt("Survived", 0);
        PlayerPrefs.Save();

        PhotonNetwork.LeaveRoom();
    }
}
