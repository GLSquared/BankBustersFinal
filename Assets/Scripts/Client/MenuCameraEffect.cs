using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Input.mousePosition.y / 80f, Input.mousePosition.x / 80f, 0), Time.deltaTime);
    }
}
