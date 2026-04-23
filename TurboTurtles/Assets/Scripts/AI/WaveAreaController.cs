/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAreaController : MonoBehaviour
{
    [SerializeField] EnemyRoundSpawner[] spawners;
    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject bridgeToLower;
    [SerializeField] float completeDelay = 3f;
    [SerializeField] WaveAreaController nextArea;

    bool hasStarted;
    bool isCompleted;
    int finishedSpawners;

    public Transform RespawnPoint => respawnPoint;
    public bool IsCompleted => isCompleted;

    public void StartArea()
    {
        if (hasStarted || isCompleted)
            return;

        hasStarted = true;
        finishedSpawners = 0;

        gamemanager.instance.SetActiveWaveArea(this);
        gamemanager.instance.ShowWaveUI(true);

        foreach (EnemyRoundSpawner spawner in spawners)
        {
            spawner.OnSpawnerFinished -= OnSpawnerFinished;
            spawner.OnSpawnerFinished += OnSpawnerFinished;
            spawner.Begin();
        }
    }

    void OnSpawnerFinished(EnemyRoundSpawner spawner)
    {
        finishedSpawners++;

        if (finishedSpawners >= spawners.Length)
        {
            StartCoroutine(CompleteAreaRoutine());
        }
    }

    IEnumerator CompleteAreaRoutine()
    {
        isCompleted = true;
        gamemanager.instance.UpdateNextWaveText("");
        yield return new WaitForSeconds(completeDelay);

        if (bridgeToLower != null)
        {
            bridgeToLower.GetComponent<DrawBridgeController>()?.LowerBridge();
        }

        gamemanager.instance.ShowWaveUI(false);

        if (nextArea == null)
            gamemanager.instance.youWin();
    }

    public void ResetArea()
    {
        hasStarted = false;
        finishedSpawners = 0;
        isCompleted = false;

        foreach (EnemyRoundSpawner spawner in spawners)
        {
            spawner.ResetSpawner();
            gamemanager.instance.SetGameGoalCount(0);
        }
    }

   
}
*/