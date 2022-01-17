using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    private int currentLevel = 0;

    [SerializeField]
    private GameObject[] Spawners;

    [SerializeField]
    private GameObject[] Objectives;

    [SerializeField] 
    private int MaximumEnemySpawns = 20;
    private int currentTotalEnemies = 0;

    Spawner Spawner;

    void Start()
    {
        Spawner = gameObject.GetComponentInChildren<Spawner>();
    }

    void FixedUpdate()
    {
        // Spawn enemies
        SpawnEnemy();
    }

    private void RestartObjectives(int level)
    {
        foreach (GameObject objective in Objectives)
        {
            if (objective.GetComponent<TriggerEventHandler>().TriggerLevel >= level)
                objective.GetComponent<BoxCollider>().enabled = true;
        } 
    }

    private void RestartGame()
    {
        currentLevel = 0;
        RestartObjectives(0);
    }

    private void RestartLevel()
    {
        RestartObjectives(currentLevel);
    }

    public void SpawnBoss(string bossName)
    {

    }

    private void SpawnEnemy()
    {
        if (currentTotalEnemies >= MaximumEnemySpawns)
            return;

        Spawner.SpawnEnemy(currentLevel);
    }

    public void InvokeTriggerEvent(string triggerName)
    {
        triggerName = triggerName.ToLower();

        switch(triggerName)
        {
            case "test": 
                print("Test called");
                break;
            case "restart": 
                print("Restart called");
                RestartGame();
                break;    
            default:
                print("Unknown event name ?"+triggerName);
                break;
        }

        print("Triggering event: "+triggerName+".");
    }
}
