using System;
using UnityEngine;

public class MarkedLocation: MonoBehaviour {
  [SerializeField] private MarkedLocationType type;

  // Public setter for type
  public void SetType(MarkedLocationType typ) => type = typ;
  public MarkedLocationType Type => type;

  private void OnDrawGizmos() {
    switch (type) {
      case MarkedLocationType.EnemySpawn:
        Gizmos.color = Color.red;
        break;
      case MarkedLocationType.CivilianSpawn:
        Gizmos.color = Color.green;
        break;
      case MarkedLocationType.Cover:
        Gizmos.color = Color.blue;
        break;
      
    }
    Gizmos.DrawWireSphere(transform.position, 0.25f);
  }
}
