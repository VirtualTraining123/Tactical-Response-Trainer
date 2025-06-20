using AI;
using JetBrains.Annotations;
using Spawner;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;

public class WristData : MonoBehaviour {
  [CanBeNull] public TextMeshProUGUI timeText;
  [SerializeField] private Transform enemyDisplayLocation;
  [SerializeField] private float enemyDisplayLocationWidth;
  [SerializeField] private KillDisplayDummy enemyDisplayPrefab;
  
  private Evaluator evaluator;
  private readonly List<KillDisplayDummy> enemyDisplays = new();

  private void Awake() {
    // Spawn a display for each enemy in the evaluator
    evaluator = Instance<Evaluator>.Get();
    var spawnManager = Instance<SpawnManager>.Get();
    var enemyCount = spawnManager.GetTotalEnemies();
    for (var i = 0; i < enemyCount;i++) {
      var enemyDisplay = Instantiate(enemyDisplayPrefab, enemyDisplayLocation);
      enemyDisplay.transform.localPosition = new((i - (enemyCount >> 1)) * enemyDisplayLocationWidth, 0, 0);
      
      enemyDisplay.name = "EnemyDisplay_" + i;
      enemyDisplays.Add(enemyDisplay);
    }
  }

  private void Update() {
    var text = "";
    text += "Tiempo: " + evaluator.GetElapsedTime().ToString("F2") + "\n";
    text += "Evaluando? " + (evaluator.isActiveAndEnabled ? "Sí" : "No") + "\n";
    if (timeText) timeText.text = text;
    // Update enemy displays based on evaluator hits

    var keys = evaluator.hits.Keys;
    for (var i = 0; i < evaluator.hits.Count; i++) {
      var enemyDisplay = enemyDisplays[i];
      if (enemyDisplay) {
        enemyDisplay.DisplayDamagedParts(evaluator.hits[keys.ElementAt(i)]);
      } else {
        Debug.LogWarning($"No display found for enemy {i}");
      }
    }
  }
}
