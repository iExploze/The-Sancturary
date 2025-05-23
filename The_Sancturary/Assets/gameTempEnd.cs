using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameTempEnd : MonoBehaviour
{
    // Start is called before the first frame update
    GameEndController gameEndController;
    void Start()
    {
        gameEndController = GetComponent<GameEndController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameEndController.TriggerGameEnd();
    }
}
