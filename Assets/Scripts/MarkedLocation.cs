using UnityEngine;

public class MarkedLocation: MonoBehaviour {
  [SerializeField] private MarkedLocationType type;

  public MarkedLocationType Type => type;
}
