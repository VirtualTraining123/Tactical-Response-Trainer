using System.Collections.Generic;
using UnityEngine;

public class CivilSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private CivilAI[] civilPrefabs; // Array para múltiples prefabs de CivilAI
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxCivilNumber;
    [SerializeField] private Player player;

    private readonly List<CivilAI> spawnedCivil = new List<CivilAI>();
    private float timeSinceLastSpawn;

    private void Start()
    {
        timeSinceLastSpawn = spawnInterval;
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > spawnInterval)
        {
            timeSinceLastSpawn = 0f;
            if (spawnedCivil.Count < maxCivilNumber)
            {
                SpawnCivil();
            }
        }
    }

    private void SpawnCivil()
    {
        // Selecciona un prefab de civil aleatoriamente
        CivilAI civilPrefab = civilPrefabs[Random.Range(0, civilPrefabs.Length)];
        CivilAI civil = Instantiate(civilPrefab, transform.position, transform.rotation);
        int spawnPointIndex = spawnedCivil.Count % spawnPoints.Length;
        civil.Init(player, spawnPoints[spawnPointIndex]);
        spawnedCivil.Add(civil);
    }
    public int GetMaxCivilNumber()
    {
        return maxCivilNumber;
    }

}
