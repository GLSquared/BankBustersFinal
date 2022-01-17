using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{

    GameObject player;

    private bool isComplete;

    GameObject canvas;

    IEnumerator WaitForCanvas() {
        yield return new WaitUntil(() => GameObject.Find("P_LPSP_UI_Canvas(Clone)") != null);
        canvas = GameObject.Find("P_LPSP_UI_Canvas(Clone)");
        canvas.SetActive(false);
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
        player.SetActive(false);
        StartCoroutine(WaitForCanvas());
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
            canvas.SetActive(true);
            player.SetActive(true);
        }
    }
}
