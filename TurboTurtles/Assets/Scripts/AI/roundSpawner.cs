using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EnemyRoundSpawner : MonoBehaviour
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
    [SerializeField] bool startOnPlay = true;
    [SerializeField] int maxWaves = 5;

    [Header("UI")]
    [SerializeField] TMP_Text waveText;
    [SerializeField] TMP_Text enemiesLeftText;
    [SerializeField] TMP_Text nextWaveText;

    int currentWave = 0;
    int enemiesAlive = 0;
    bool waveInProgress = false;

    void Start()
    {
        UpdateWaveUI();
        UpdateEnemiesLeftUI();
        UpdateNextWaveUI("");

        if (startOnPlay)
        {
            StartCoroutine(WaveLoop());
        }
    }

    IEnumerator WaveLoop()
    {
        if (startDelay > 0)
        {
            yield return StartCoroutine(CountdownRoutine(startDelay));
        }

        while (currentWave < maxWaves)
        {
            currentWave++;
            UpdateWaveUI();

            yield return StartCoroutine(SpawnWave());

            waveInProgress = true;

            yield return new WaitUntil(() => enemiesAlive <= 0);

            waveInProgress = false;

            if (currentWave < maxWaves)
            {
                yield return StartCoroutine(CountdownRoutine(timeBetweenWaves));
            }
        }

        if (gamemanager.instance != null)
        {
            gamemanager.instance.youWin();
        }
    }

    IEnumerator SpawnWave()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned.");
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

        UpdateEnemiesLeftUI();
        UpdateNextWaveUI("");
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        EnemySpawnPoint point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Transform spawnT = point.GetSpawnTransform();

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnT.position, spawnT.rotation);

        EnemyHealth enemyHealth = spawnedEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.SetSpawner(this);
        }

        enemiesAlive++;
        UpdateEnemiesLeftUI();
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

    public void NotifyEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
            enemiesAlive = 0;

        UpdateEnemiesLeftUI();
    }

    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            if (currentWave == maxWaves)
                waveText.text = "Final Wave!";
            else
                waveText.text = "Wave: " + currentWave;
        }
    }

    void UpdateEnemiesLeftUI()
    {
        if (enemiesLeftText != null)
        {
            enemiesLeftText.text = "Enemies Left: " + enemiesAlive;
        }
    }

    void UpdateNextWaveUI(string message)
    {
        if (nextWaveText != null)
        {
            nextWaveText.text = message;
        }
    }
}