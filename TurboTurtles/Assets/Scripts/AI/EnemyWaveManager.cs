using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject enemyPrefab;
        public int startingAmount = 3;
        public int increasePerWave = 1;
    }

    [Header("Spawn Points")]
    [SerializeField] EnemySpawnPoint[] spawnPoints;

    [Header("Enemies")]
    [SerializeField] List<EnemySpawnEntry> enemyTypes = new List<EnemySpawnEntry>();

    [Header("Wave Settings")]
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] float startDelay = 3f;
    [SerializeField] int maxWaves = 5;

    [Header("Area Settings")]
    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject bridgeToLower;
    [SerializeField] float completeDelay = 3f;
    [SerializeField] EnemyWaveManager nextArea;
    [SerializeField] bool startOnTrigger = true;
    [SerializeField] bool startOnPlay = false;

    int currentWave;
    bool hasStarted;
    bool isCompleted;

    Coroutine waveRoutine;
    List<GameObject> spawnedEnemies = new List<GameObject>();

    public Transform RespawnPoint => respawnPoint;

    void Start()
    {
        if (startOnPlay)
            StartArea();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!startOnTrigger) return;
        if (!other.CompareTag("Player")) return;

        StartArea();
    }

    public void StartArea()
    {
        if (hasStarted || isCompleted)
            return;

        hasStarted = true;
        currentWave = 0;

        gamemanager.instance.SetActiveWaveArea(this);
        gamemanager.instance.ShowWaveUI(true);
        gamemanager.instance.SetGameGoalCount(0);

        waveRoutine = StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        if (startDelay > 0)
            yield return StartCoroutine(CountdownRoutine(startDelay));

        while (currentWave < maxWaves)
        {
            currentWave++;
            UpdateWaveUI();

            yield return StartCoroutine(SpawnWave());

            yield return new WaitUntil(() => CountLivingSpawnedEnemies() <= 0);

            if (currentWave < maxWaves)
                yield return StartCoroutine(CountdownRoutine(timeBetweenWaves));
        }

        StartCoroutine(CompleteAreaRoutine());
    }

    IEnumerator SpawnWave()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning(gameObject.name + " has no spawn points assigned.");
            yield break;
        }

        foreach (EnemySpawnEntry entry in enemyTypes)
        {
            if (entry.enemyPrefab == null)
                continue;

            int amountToSpawn = entry.startingAmount + ((currentWave - 1) * entry.increasePerWave);

            for (int i = 0; i < amountToSpawn; i++)
            {
                SpawnEnemy(entry.enemyPrefab);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        UpdateNextWaveUI("");
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        EnemySpawnPoint point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Transform spawnT = point.GetSpawnTransform();

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnT.position, spawnT.rotation);
        spawnedEnemies.Add(spawnedEnemy);
    }

    IEnumerator CountdownRoutine(float duration)
    {
        float timer = duration;

        while (timer > 0)
        {
            UpdateNextWaveUI("Next Wave: " + Mathf.CeilToInt(timer));
            timer -= Time.deltaTime;
            yield return null;
        }

        UpdateNextWaveUI("");
    }

    IEnumerator CompleteAreaRoutine()
    {
        isCompleted = true;
        UpdateNextWaveUI("");

        yield return new WaitForSeconds(completeDelay);

        if (bridgeToLower != null)
            bridgeToLower.GetComponent<DrawBridgeController>()?.LowerBridge();

        gamemanager.instance.ShowWaveUI(false);

        if (nextArea == null)
            gamemanager.instance.youWin();
    }

    int CountLivingSpawnedEnemies()
    {
        int livingCount = 0;

        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
                spawnedEnemies.RemoveAt(i);
            else
                livingCount++;
        }

        return livingCount;
    }

    void UpdateWaveUI()
    {
        if (gamemanager.instance == null) return;

        if (currentWave == maxWaves)
            gamemanager.instance.UpdateWaveText("Final Wave!");
        else
            gamemanager.instance.UpdateWaveText("Wave: " + currentWave);
    }

    void UpdateNextWaveUI(string message)
    {
        if (gamemanager.instance != null)
            gamemanager.instance.UpdateNextWaveText(message);
    }

    public void ResetArea()
    {
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();

        currentWave = 0;
        hasStarted = false;
        isCompleted = false;

        gamemanager.instance.SetGameGoalCount(0);
        gamemanager.instance.UpdateWaveText("");
        gamemanager.instance.UpdateNextWaveText("");
    }

}
