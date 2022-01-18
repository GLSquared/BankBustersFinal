using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedSpawns : MonoBehaviour
{
    [SerializeField]
    GameObject doorLink;
    [SerializeField]
    int actualLevel;

    public bool canSpawn = false;

    Door door;
    Spawn spawn;

    void Start()
    {
        spawn = GetComponent<Spawn>();
        canSpawn = false;
        door = doorLink.GetComponent<Door>();
    }

    void Update()
    {
        if (door.isOpen)
        {
            canSpawn = true;
        } else
        {
            canSpawn = false;
        }
    } 
}
