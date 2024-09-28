using System.Collections.Generic;
using AI;
using UnityEngine;

namespace Spawner {
  public class SpawnManager : MonoBehaviour {
    [SerializeField] private GameObject spawnPoints;
    [SerializeField] private List<RunToTargetAI> enemies;
    [SerializeField] private List<RunToTargetAI> civilians;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int civiliansToSpawn;
    private readonly List<MarkedLocation> enemyLocations = new();
    private readonly List<MarkedLocation> civilianLocations = new();
    private readonly List<MarkedLocation> coverLocations = new();

    private void Awake() {
      Debug.Log("Awake " + spawnPoints.transform.childCount);
      for (var i = 0; i < spawnPoints.transform.childCount; i++) {
        var child = spawnPoints.transform.GetChild(i);
        var markedLocation = child.GetComponent<MarkedLocation>();
        switch (markedLocation.Type) {
          case MarkedLocationType.EnemySpawn:
            enemyLocations.Add(markedLocation);
            break;
          case MarkedLocationType.CivilianSpawn:
            civilianLocations.Add(markedLocation);
            break;
          case MarkedLocationType.Cover:
            coverLocations.Add(markedLocation);
            break;
        }
        
      }
      Debug.Log("Enemy locations: " + enemyLocations.Count);
      Debug.Log("Civilian locations: " + civilianLocations.Count);
      SpawnRunToTargetAI(enemiesToSpawn, enemyLocations, enemies);
      SpawnRunToTargetAI(civiliansToSpawn, civilianLocations, civilians);
    }

    private void SpawnRunToTargetAI(int toSpawn, List<MarkedLocation> markedLocations, List<RunToTargetAI> prefabs) {
      for (var i = 0; i < toSpawn; i++) {
        var selectedId = Random.Range(0, prefabs.Count);
        var selectedTransformId = Random.Range(0, markedLocations.Count);
        var prefab = Instantiate(prefabs[selectedId], markedLocations[selectedTransformId].transform);
        prefab.SetMarkedLocations(coverLocations.ToArray());
        Debug.Log($"Spawned {prefab.name} at {markedLocations[selectedTransformId].transform.position}");
        markedLocations.RemoveAt(selectedTransformId);
      }
    }

    public int GetTotalEnemies() => enemiesToSpawn;
    public int GetTotalCivilians() => civiliansToSpawn;
  }
}