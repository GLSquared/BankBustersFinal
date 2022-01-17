using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] EnemyPrefabs;

    List<GameObject>[] SpawnerLevels;

    void Start()
    {
        SpawnerLevels = new List<GameObject>[3];

        for (int index = 0; index < SpawnerLevels.Length; index++)
        {
            SpawnerLevels[index] = new List<GameObject>();
        }

        foreach(GameObject spawner in GameObject.FindGameObjectsWithTag("SpawnLocation"))
        {
            var spawn = spawner.GetComponent<Spawn>();
            var level = spawn.level;

            SpawnerLevels[level].Add(spawner);
        }
    }

    public void SpawnEnemy(int level)
    {
        GameObject[] levelSpawners = SpawnerLevels[level].ToArray();
        GameObject randomSpawner = levelSpawners[Random.Range(0, levelSpawners.Length)];

        GameObject newEnemy = Instantiate(EnemyPrefabs[level], randomSpawner.transform.position, Quaternion.identity);
        // newEnemy.transform.position = randomSpawner.transform.position;
        newEnemy.GetComponent<Enemy>().currentTarget = GameObject.Find("Player");
    }
}
