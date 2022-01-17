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
    public Quaternion rotationClose;
    public Quaternion rotationOpen;

    void Start()
    {
        // rotationClose = transform.localRotation;
    }

    public IEnumerator HackDoor(float timer)
    {
        float maxFrames = Mathf.Floor(timer / Time.deltaTime);

        for (int i = 0; i < maxFrames; i++) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (isOpen)
        {
            for (int i = 0; i < 100; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    rotationClose, 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            isOpen = false;
        } else if (!isOpen)
        {   
            for (int i = 0; i < 100; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    rotationOpen, 
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
            for (int i = 0; i < 100; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    rotationClose, 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            isOpen = false;
        } else if (!isOpen){
            for (int i = 0; i < 100; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    rotationOpen, 
                    Time.deltaTime * 30);
                transform.localRotation = rot;
                yield return new WaitForSeconds(Time.deltaTime);
            } 
            isOpen = true;
        }
    }
}
