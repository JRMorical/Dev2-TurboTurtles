using UnityEngine;

public class DeathDropSpawner : MonoBehaviour
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject dropPrefab;
        [Range(0f, 1f)] public float dropChance = 0.15f;
    }

    [Header("Drops")]
    [SerializeField] DropEntry[] possibleDrops;

    [Header("Spawn Settings")]
    [SerializeField] Transform dropSpawnPoint;
    [SerializeField] bool useRandomDrop = true;

    public static bool suppressDrops;

    bool applicationQuitting;

    private void OnApplicationQuit()
    {
        applicationQuitting = true;
    }

    private void OnDestroy()
    {
        if (applicationQuitting) return;
        if (suppressDrops) return;

        TrySpawnDrop();
    }

    void TrySpawnDrop()
    {
        if (possibleDrops == null || possibleDrops.Length == 0)
            return;

        Vector3 spawnPos = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position;

        if (useRandomDrop)
        {
            DropEntry chosenDrop = possibleDrops[Random.Range(0, possibleDrops.Length)];
            TrySpawnSingleDrop(chosenDrop, spawnPos);
        }
        else
        {
            foreach (DropEntry drop in possibleDrops)
            {
                TrySpawnSingleDrop(drop, spawnPos);
            }
        }
    }

    void TrySpawnSingleDrop(DropEntry drop, Vector3 spawnPos)
    {
        if (drop == null || drop.dropPrefab == null)
            return;

        if (Random.value <= drop.dropChance)
        {
            Instantiate(drop.dropPrefab, spawnPos, Quaternion.identity);
        }
    }
}
