using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Transform Target;
    public int numberOfEnemiesToSpawn = 5;
    public float spawnDelay = 1f;
    public List<Enemy> enemyPrefabs = new List<Enemy>();
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation triangulation;
    public Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

    private void Awake()
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(enemyPrefabs[i], numberOfEnemiesToSpawn));
        }
    }


    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        int spawnedEnemies = 0;

        while (spawnedEnemies < numberOfEnemiesToSpawn)
        {
            if (EnemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if (EnemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }

            spawnedEnemies++;

            yield return wait;
        }
    }

    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int spawnIndex = spawnedEnemies % enemyPrefabs.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, enemyPrefabs.Count));
    }

    private void DoSpawnEnemy(int spawnIndex)
    {
        PoolableObject poolableObject = EnemyObjectPools[spawnIndex].GetObject();

        if (poolableObject != null)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();

            int vertexIndex = Random.Range(0, triangulation.vertices.Length);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, -1))
            {
                enemy.Agent.Warp(hit.position);
                // Enemy needs to get enabled and start chasing now
                enemy.Movement.Target = Target;
                enemy.Agent.enabled = true;
                enemy.Movement.StartChasing();
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on Navmesh. Tried to use {triangulation.vertices[vertexIndex]}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy og type {spawnIndex} from object pool. Out of objects?");
        }
    }

    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }
}
