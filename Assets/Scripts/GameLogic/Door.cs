using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    public string TriggerDoor;
    public int TriggerLevel;
    public Quaternion rotation;

    void Start()
    {
        rotation = transform.localRotation;
    }
}
