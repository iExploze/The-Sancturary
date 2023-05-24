using UnityEngine;

public class HeartbeatController : MonoBehaviour
{
    private GameObject player;
    public string monsterTag = "Monster";
    public string monsterTag2 = "MonsterNoHeartBeat";
    public float maxDistance = 10f;
    public AudioSource heartbeatAudioSource;
    public AudioSource scaryMusicAudioSource;
    public AnimationCurve volumeCurve;
    public AnimationCurve pitchCurve;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag(monsterTag);
        float minDistance = maxDistance;

        foreach (GameObject monster in monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        float t = 1f - Mathf.Clamp01(minDistance / maxDistance);
        heartbeatAudioSource.volume = volumeCurve.Evaluate(t);
        heartbeatAudioSource.pitch = pitchCurve.Evaluate(t);

        if (isPlayerChased())
        {
            if (!scaryMusicAudioSource.isPlaying)
            {
                scaryMusicAudioSource.Play();
            }
        }
        else
        {
            if (scaryMusicAudioSource.isPlaying)
            {
                scaryMusicAudioSource.Stop();
            }
        }
    }

    private bool isPlayerChased()
    {
        return player.GetComponent<PlayerMovement>().isChased;
    }
}
