using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    Vector3 originPos;
    // Start is called before the first frame update
    void Start()
    {
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = originPos + new Vector3(0, Mathf.Sin(Time.time*2)/10, 0);
        transform.rotation = transform.rotation * Quaternion.Euler(Time.deltaTime*8f, Time.deltaTime * 14f, 0);
    }
}
