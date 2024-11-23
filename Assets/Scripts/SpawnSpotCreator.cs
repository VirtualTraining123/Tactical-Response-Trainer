using System.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class SpawnSpotItem {
    public MarkedLocationType type;
    public int count;
}

public class SpawnSpotCreator : MonoBehaviour
{
    [SerializeField] private GameObject spawnSpots;
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;
    
    public SpawnSpotItem[] nPoints;

    private Vector3 SpawnAreaCenter => spawnAreaCenter + transform.position;
    private Vector3 SpawnAreaSize => spawnAreaSize;

    private void Awake() {
        Array.ForEach(nPoints, item => CreateSpawnSpots(item.type, item.count));
        
    }

    private void CreateSpawnSpots(MarkedLocationType itemType, int itemCount) {
        for (var i = 0; i < itemCount; i++) {
            var spot = PickRandomSpot(i);
            var ml = spot.AddComponent<MarkedLocation>();
            ml.SetType(itemType);
        }
    }

    private GameObject PickRandomSpot(int i) {
        var x = UnityEngine.Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        var y = UnityEngine.Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        var z = UnityEngine.Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);
        var point = new Vector3(x, y, z) + SpawnAreaCenter;
            
        // Map the point to the navmesh
        if (NavMesh.SamplePosition(point, out var hit, 100.0f, NavMesh.AllAreas)) {
            point = hit.position;
        }
            
        var spot = new GameObject("Spot-" + i) {
            transform = {
                position = point,
                parent = spawnSpots.transform
            }
        };
        return spot;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SpawnAreaCenter, SpawnAreaSize);
    }
}
