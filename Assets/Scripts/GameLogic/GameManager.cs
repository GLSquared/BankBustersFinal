using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    // Level
    public int currentLevel = 0;

    // Enemy Spawners
    [SerializeField]
    private GameObject[] Spawners;
    private bool enemiesAlerted = false;

    // Objectives (REMEMBER TO ADD THE TRIGGERS TO THE OBJECTIVE FIELD BELOW)
    [SerializeField]
    private GameObject[] Objectives;

    // Maximum enemy spawns
    [SerializeField] 
    private int MaximumEnemySpawns = 5;

    // Spawner
    Spawner Spawner;
    
    // Player
    private GameObject player;

    void Start()
    {
        Spawner = gameObject.GetComponentInChildren<Spawner>();
        player = GameObject.FindWithTag("Player");
    }

    void FixedUpdate()
    {
        // Enemy update
        if (enemiesAlerted && GameObject.FindGameObjectsWithTag("Enemy").Length < MaximumEnemySpawns)
        {
            Spawner.SpawnEnemy(currentLevel);
        }
    }

    private void ResetGame(int level)
    {
        currentLevel = level;

        // Reset objectives
        foreach (GameObject objective in Objectives)
        {
            if (objective.GetComponent<TriggerEventHandler>().TriggerLevel >= level)
                objective.GetComponent<BoxCollider>().enabled = true;
        }

        // Reset doors
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (door.GetComponent<Door>().TriggerLevel >= level)
            {
                door.transform.localRotation = door.GetComponent<Door>().rotation;
                door.GetComponent<Door>().isOpen = false;
            }
        }     

        // Eliminate enemies
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        // Respawn Player
        player.transform.position = FindPlayerRespawnLocation(level);
    }

    // Hacking doors
    private void HackDoor(string trigger, float timer)
    {
        if (player && !player.GetComponent<ClientController>().Dead)
            player.GetComponent<ClientController>().ShowHackingProgress(timer);

        GameObject door = FindDoorOfTrigger(trigger);
        if (door)
            StartCoroutine(door.GetComponent<Door>().HackDoor(timer));
    }

    // Open doors
    private void OpenDoor(string trigger)
    {
        GameObject door = FindDoorOfTrigger(trigger);
        if (door)
            StartCoroutine(door.GetComponent<Door>().OpenDoor());
    }

    // Search for the door 
    private GameObject FindDoorOfTrigger(string trigger)
    {
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (door.GetComponent<Door>().TriggerDoor.Equals(trigger))
                return door;
        }

        return null;
    }

    // Search for a respawn location 
    private Vector3 FindPlayerRespawnLocation(int level)
    {
        foreach (GameObject spawn in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            if (spawn.GetComponent<PlayerSpawn>().Level == level)
                return spawn.transform.position;
        }

        return Vector3.zero;
    }

    // Cheat to eliminate enemies
    private void Nuke()
    {
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Enemy>().Dead();
        }
    }

    // Invoke trigger boxes events
    public void InvokeTriggerEvent(string triggerName)
    {
        triggerName = triggerName.ToLower();

        switch(triggerName)
        {
            //CHEAT
            case "nuke": 
                Nuke();
                break;
            case "gamestart":
                enemiesAlerted = true;
                break;
            case "gameover":
                ResetGame(0);
                break;
            case "restartlevel":
                ResetGame(currentLevel);
                break;
            case "":
                break;
            case "hackdoor":
                HackDoor(triggerName, 5f);
                break;
            case "opendoor":
                OpenDoor(triggerName);
                break;
            default:
                Debug.Log("Unknown Trigger event name '"+triggerName+"'");
                break;

            /*
            case "opentestdoor":
                StartCoroutine(OpenDoor(FindDoorOfTrigger(triggerName)));
                break;
            case "hacktestdoor":
                StartCoroutine(HackDoor(FindDoorOfTrigger(triggerName), 5f));
                break;
             */
        }
    }
}
