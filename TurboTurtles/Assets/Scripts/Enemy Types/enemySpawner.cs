using UnityEngine;
using System.Collections.Generic;

public class enemySpawner : MonoBehaviour, IEnemyBehaviour
{
    [Header("-----Spawner Stats-----")]
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] float spawnCooldown = 5f;
    [SerializeField] float spawnRadius = 8f;
    [SerializeField] int maxSpawned = 3;
    [SerializeField] float spawnRangeMax = 4f;
    [SerializeField] float spawnRangeMin = -4f;
    int currentSpawned;
    float spawnTimer;

    void TrySpawn()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.Log("LIST.COUNT = 0, ADD ENEMY PREFABS TO YOUR LIST");
            return;
        }

        currentSpawned++;

        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        Vector3 offset = Random.insideUnitSphere * spawnRadius;
        offset.y = 0;

        Vector3 pos = transform.position + offset + Vector3.right * Random.Range(spawnRangeMin, spawnRangeMax); ;

        GameObject enemy = Instantiate(prefabToSpawn, pos, transform.rotation);
        spawnTimer = 0;
        enemyAI ai = enemy.GetComponent<enemyAI>();
        ai.SetSpawner(this);
    }
    public void SpawnDeath()
    {
        --currentSpawned;
    }
    public void ResetTimer()
    {
        spawnTimer = 0;
    }
    public void Tick(enemyAI _ai)
    {
        spawnTimer += Time.deltaTime;
        if (currentSpawned >= maxSpawned) return;
        if (spawnTimer >= spawnCooldown) TrySpawn();
    }
}
