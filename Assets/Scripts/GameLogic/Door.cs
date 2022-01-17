using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Door properties
    public bool isOpen = false;
    public string TriggerDoor;
    public int TriggerLevel;

    // Door local rotation
    public Quaternion rotation;

    void Start()
    {
        rotation = transform.localRotation;
    }

    public IEnumerator HackDoor(float timer)
    {
        float maxFrames = Mathf.Floor(timer / Time.deltaTime);

        for (int i = 0; i < maxFrames; i++) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (isOpen)
        {
            for (int i = 0; i < 150; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    rotation, 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            isOpen = false;
        } else if (!isOpen)
        {   
            for (int i = 0; i < 150; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    Quaternion.Euler(0, transform.localRotation.y + (10 * 0.5f), 0), 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            } 
            isOpen = true;
        }
    }

    public IEnumerator OpenDoor()
    {
        if (isOpen)
        {
            for (int i = 0; i < 150; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    rotation, 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            isOpen = false;
        } else if (!isOpen){
            for (int i = 0; i < 150; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    Quaternion.Euler(0, transform.localRotation.y + (10 * 0.5f), 0), 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            } 
            isOpen = true;
        }
    }
}
