using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private GameObject towerPrefab;

    [SerializeField] private Vector3 spawnZoneCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnZoneSize = new Vector3(4f, 0.5f, 4f);
    [SerializeField] private bool spawnOnStart = false;

    private GameObject spawnedTower;
    private int ballsUsedThisStructure = 0;
    private int maxBallsPerStructure = 3;
    private bool structureDestroyed = false;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnTower();
        }
    }

    public void SpawnTower()
    {
        if (platform == null)
        {
            Debug.LogError("Platform no asignada en el inspector");
            return;
        }

        if (towerPrefab == null)
        {
            Debug.LogError("Tower Prefab no asignado en el inspector");
            return;
        }

        if (spawnedTower != null)
        {
            Destroy(spawnedTower);
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnZoneSize.x / 2f, spawnZoneSize.x / 2f),
            0,
            Random.Range(-spawnZoneSize.z / 2f, spawnZoneSize.z / 2f)
        );

        Vector3 spawnPosition = platform.position + spawnZoneCenter + randomOffset;
        spawnedTower = Instantiate(towerPrefab, spawnPosition, Quaternion.identity);

        ballsUsedThisStructure = 0;
        structureDestroyed = false;

        Debug.Log($"Torre spawneada en posición: {spawnPosition}");
    }

    public void OnBallThrown()
    {
        ballsUsedThisStructure++;
        Debug.Log($"Bolas usadas en esta estructura: {ballsUsedThisStructure}/{maxBallsPerStructure}");
        CheckIfBallsExhausted();
    }

    public void OnStructureDestroyed()
    {
        structureDestroyed = true;
        Debug.Log("Estructura completamente destruida. Presiona ESPACIO para resetear.");
    }

    public void CheckIfBallsExhausted()
    {
        if (ballsUsedThisStructure >= maxBallsPerStructure && !structureDestroyed)
        {
            Debug.Log("Se acabaron las bolas para esta estructura. Presiona ESPACIO para resetear.");
        }
    }

    public void ResetForNextShot()
    {
        SpawnTower();
    }

    public void DestroyTower()
    {
        if (spawnedTower != null)
        {
            Destroy(spawnedTower);
            spawnedTower = null;
            Debug.Log("Torre destruida");
        }
    }

    public int GetBallsUsedThisStructure()
    {
        return ballsUsedThisStructure;
    }

    public int GetMaxBallsPerStructure()
    {
        return maxBallsPerStructure;
    }

    private void OnDrawGizmos()
    {
        if (platform == null) return;

        Vector3 zoneCenter = platform.position + spawnZoneCenter;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(zoneCenter, spawnZoneSize);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(zoneCenter, 0.2f);
    }
}
