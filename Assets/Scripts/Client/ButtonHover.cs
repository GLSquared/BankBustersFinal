using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void bruh()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }

    public void EnterMainScene() {
        SceneManager.LoadScene("FullMap");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
