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
    public Vector3 rotationClose;
    public Vector3 rotationOpen;

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
            for (int i = 0; i < 200; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation, 
                    Quaternion.Euler(rotationClose.x, rotationClose.y, rotationClose.z), 
                    Time.deltaTime * 2f);
                transform.localRotation = rot;
                yield return new WaitForEndOfFrame();
            }
            isOpen = false;
        } else if (!isOpen)
        {   
            for (int i = 0; i < 200; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation,
                    Quaternion.Euler(rotationOpen.x, rotationOpen.y, rotationOpen.z), 
                    Time.deltaTime * 2f);
                transform.localRotation = rot;
                yield return new WaitForEndOfFrame();
            } 
            isOpen = true;
        }
    }

    public IEnumerator OpenDoor()
    {
        if (isOpen)
        {
            for (int i = 0; i < 200; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation,
                    Quaternion.Euler(rotationClose.x, rotationClose.y, rotationClose.z), 
                    Time.deltaTime * 2f);
                transform.localRotation = rot;
                yield return new WaitForEndOfFrame();
            }
            isOpen = false;
        } else if (!isOpen){
            for (int i = 0; i < 200; i++)
            {
                Quaternion rot = Quaternion.Lerp(transform.localRotation,
                    Quaternion.Euler(rotationOpen.x, rotationOpen.y, rotationOpen.z), 
                    Time.deltaTime * 2f);
                transform.localRotation = rot;
                yield return new WaitForEndOfFrame();
            } 
            isOpen = true;
        }
    }
}
