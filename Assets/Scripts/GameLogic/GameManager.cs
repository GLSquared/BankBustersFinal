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

    // Objectives (REMEMBER TO ADD THE TRIGGERS TO THE OBJECTIVE FIELD BELOW)
    [SerializeField]
    private GameObject[] Objectives;

    // Maximum enemy spawns
    [SerializeField] 
    private int MaximumEnemySpawns = 5;

    // Spawner
    Spawner Spawner;
    
    private GameObject currentPlayerController;

    void Start()
    {
        Spawner = gameObject.GetComponentInChildren<Spawner>();
    }

    void FixedUpdate()
    {
        // Spawn enemies
        SpawnEnemy();

        currentPlayerController = GameObject.FindWithTag("Player");
    }

    // Restart game objectives
    private void RestartObjectives(int level)
    {
        foreach (GameObject objective in Objectives)
        {
            if (objective.GetComponent<TriggerEventHandler>().TriggerLevel >= level)
                objective.GetComponent<BoxCollider>().enabled = true;
        }
        RestartDoors();
    }

    public void RespawnPlayerToLevel(int level)
    {
        switch(level)
            {
                case 0: 
                    currentPlayerController.transform.position = GameObject.FindGameObjectWithTag("Respawn0").transform.position;
                    break;
                case 1:
                    currentPlayerController.transform.position = GameObject.FindGameObjectWithTag("Respawn1").transform.position;
                    break;
                case 2:
                    currentPlayerController.transform.position = GameObject.FindGameObjectWithTag("Respawn2").transform.position;
                    break;
                default:
                    print("Unknown event name respawn");
                    break;
            }  
    }

    private void RestartEnemies()
    {
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    // Restart Game
    private void RestartGame()
    {
        currentLevel = 0;
        RestartObjectives(0);
        RestartDoors();
        RespawnPlayerToLevel(0);
        RestartEnemies();
    }

    // Reset the doors to closed
    private void RestartDoors()
    {
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (door.GetComponent<Door>().TriggerLevel >= currentLevel)
            {
                door.transform.localRotation = door.GetComponent<Door>().rotation;
                door.GetComponent<Door>().isOpen = false;
            }
        }        
    }

    // Restart level
    private void RestartLevel()
    {
        RestartObjectives(currentLevel);
        RespawnPlayerToLevel(currentLevel);
        RestartEnemies();
    }

    // Spawn enemies
    private void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < MaximumEnemySpawns)
            Spawner.SpawnEnemy(currentLevel);
    }

    // Open level doors
    IEnumerator OpenDoor(GameObject door)
    {
        print(door.GetComponent<Door>().isOpen);

        if (door.GetComponent<Door>().isOpen)
        {
            for (int i = 0; i < 150; i++)
            {
                Quaternion nah = Quaternion.Lerp(door.transform.localRotation, door.GetComponent<Door>().rotation, Time.deltaTime * 30);
                door.transform.localRotation = nah;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            door.GetComponent<Door>().isOpen = false;
        } else if (!door.GetComponent<Door>().isOpen){
            for (int i = 0; i < 150; i++)
            {
                Quaternion nah = Quaternion.Lerp(door.transform.localRotation, Quaternion.Euler(0, door.transform.localRotation.y + (10 * 0.5f), 0), Time.deltaTime * 30);
                door.transform.localRotation = nah;
                yield return new WaitForSeconds(Time.deltaTime);
            } 
            door.GetComponent<Door>().isOpen = true;
        }
    }

    // Hack level doors
    IEnumerator HackDoor(GameObject door, float timer)
    {
        if (currentPlayerController)
            currentPlayerController.GetComponent<ClientController>().ShowHackingProgress(timer);

        float maxFrames = Mathf.Floor(timer / Time.deltaTime);
        for (int i = 0; i < maxFrames; i++) {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (door.GetComponent<Door>().isOpen)
        {
            for (int i = 0; i < 150; i++)
            {
                Quaternion nah = Quaternion.Lerp(door.transform.localRotation, door.GetComponent<Door>().rotation, Time.deltaTime * 30);
                door.transform.localRotation = nah;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            door.GetComponent<Door>().isOpen = false;
        } else if (!door.GetComponent<Door>().isOpen){
            for (int i = 0; i < 150; i++)
            {
                Quaternion nah = Quaternion.Lerp(door.transform.localRotation, Quaternion.Euler(0, door.transform.localRotation.y + (10 * 0.5f), 0), Time.deltaTime * 30);
                door.transform.localRotation = nah;
                yield return new WaitForSeconds(Time.deltaTime);
            } 
            door.GetComponent<Door>().isOpen = true;
        }
    }

    private GameObject FindDoorOfTrigger(string trigger)
    {
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (door.GetComponent<Door>().TriggerDoor.Equals(trigger))
                return door;
        }

        return null;
    }

    // Invoke trigger boxes events
    public void InvokeTriggerEvent(string triggerName)
    {
        triggerName = triggerName.ToLower();

        switch(triggerName)
        {
            case "nuke": 
                foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<Enemy>().Dead();
                }
                break;
            case "opentestdoor":
                StartCoroutine(OpenDoor(FindDoorOfTrigger(triggerName)));
                break;
            case "hacktestdoor":
                StartCoroutine(HackDoor(FindDoorOfTrigger(triggerName), 5f));
                break;
            case "onrespawn0":
                print("player is on respawn trigger");
                break;
            case "level1complete":
                if (currentLevel == 0)
                    currentLevel = 1;
                break;
            case "level2complete":
                if (currentLevel == 1)
                    currentLevel = 2;
                break;
            case "level3complete":
                if (currentLevel == 3)
                    print("prosit irbaht");
                break;                
            case "restart": 
                print("Restart called");
                RestartGame();
                break;   
            case "restartlevel": 
                RestartLevel();
                break;                     
            default:
                print("Unknown event name ?"+triggerName);
                break;
        }

        print("Triggering event: "+triggerName+".");
    }
}
