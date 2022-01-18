using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private GameObject Helicopter;

    // Maximum enemy spawns
    [SerializeField] 
    private int MaximumEnemySpawns = 5;

    [SerializeField]
    private GameObject[] Bosses;

    // Spawner
    Spawner Spawner;
    
    // Player
    [SerializeField]
    private GameObject player;

    private IEnumerator hackCoroutine;

    void Start()
    {
        Helicopter.SetActive(false);
        Spawner = gameObject.GetComponentInChildren<Spawner>();
        // player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        // Enemy update
        if (enemiesAlerted && GameObject.FindGameObjectsWithTag("Enemy").Length < MaximumEnemySpawns)
        {
            Spawner.SpawnEnemy(currentLevel);
        }
    }

    void SpawnBoss(int level)
    {
        GameObject spawn = GameObject.Find("BossSpawn"+level.ToString());

        MaximumEnemySpawns += 2;

        GameObject newEnemy = Instantiate(Bosses[level], spawn.transform.position, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().currentTarget = GameObject.Find("Player");
    }

    private void ResetGame(int level)
    {
        currentLevel = level;
        
        if (level == 0)
            enemiesAlerted = false;

        // Reset objectives
        foreach (GameObject objective in Objectives)
        {
            if (objective.GetComponent<TriggerEventHandler>().TriggerLevel >= level)
                objective.GetComponent<BoxCollider>().enabled = true;
        }

        // Stop hacking!
        if (hackCoroutine != null)
        {
            StopCoroutine(hackCoroutine);
            hackCoroutine = null;
        }

        // Reset doors
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (door.GetComponent<Door>().TriggerLevel >= level)
            {
                door.transform.localRotation = Quaternion.Euler(door.GetComponent<Door>().rotationClose.x, door.GetComponent<Door>().rotationClose.y, door.GetComponent<Door>().rotationClose.z);
                door.GetComponent<Door>().isOpen = false;
            }
        }     

        // Eliminate enemies
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            //IF enemy is not a boss
            // if (!enemy.name.Contains("Boss")) {
            Destroy(enemy);
            // }
        }

        print(player);

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
        {
            hackCoroutine = door.GetComponent<Door>().HackDoor(timer);
            StartCoroutine(hackCoroutine);
        }
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
    public void Nuke()
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
                foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<Enemy>().currentTarget = player;
                }
                break;
            case "gameover":
                ResetGame(0);
                break;
            case "restartlevel":
                ResetGame(currentLevel);
                break;
            case "saferoom1":
                OpenDoor(triggerName);
                break;
            case "saferoom2":
                OpenDoor(triggerName);
                break;
            case "saferoom3":
                OpenDoor(triggerName);
                break;
            case "boss_1":
                SpawnBoss(currentLevel);
                break;
            case "bosshack1":
                HackDoor(triggerName, 15f);
                SpawnBoss(0);
                break;
            case "bosshack2":
                HackDoor(triggerName, 15f);
                SpawnBoss(1);
                break;
            case "bosshack3":
                HackDoor(triggerName, 15f);
                SpawnBoss(2);
                Helicopter.SetActive(true);
                break;
            case "opendoor":
                OpenDoor(triggerName);
                break;
            case "escape":
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                Helicopter.GetComponent<HelicopterAnim>().animPlaying = true;
                StartCoroutine(waitForAnimation(20f));
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

    IEnumerator waitForAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = (Cursor.visible == false ? CursorLockMode.Locked : CursorLockMode.None);
        SceneManager.LoadScene("MainMenu");
    }
}
