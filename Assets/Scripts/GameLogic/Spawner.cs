using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] EnemyPrefabs;

    List<GameObject>[] SpawnerLevels = new List<GameObject>[3];

    public void Start()
    {
        SpawnerLevels = new List<GameObject>[3];

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
        print(levelSpawners);
        GameObject randomSpawner = levelSpawners[Random.Range(0, levelSpawners.Length)];

        GameObject newEnemy = Instantiate(EnemyPrefabs[level]);
        newEnemy.transform.position = randomSpawner.transform.position;
    }
}
