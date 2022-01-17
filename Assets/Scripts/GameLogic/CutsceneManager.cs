using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{

    GameObject player;

    private bool isComplete;

    private void Awake()
    {
        player = GameObject.Find("Player");
        player.SetActive(false);
    }

    private void Update()
    {
        if (!isComplete)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("CutsceneCam");

            if (cam.GetComponent<PlayableDirector>().state == PlayState.Paused)
            {
                isComplete = true;
            }
        }
        else
        {
            player.SetActive(true);
        }
    }
}
