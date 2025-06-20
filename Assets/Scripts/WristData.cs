using AI;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WristData : MonoBehaviour {
  [CanBeNull] public TextMeshProUGUI timeText;
  [SerializeField] private Evaluator evaluator;
  [SerializeField] private Transform enemyDisplayLocation;
  [SerializeField] private float enemyDisplayLocationWidth;
  [SerializeField] private KillDisplayDummy enemyDisplayPrefab;
  
  private List<KillDisplayDummy> enemyDisplays = new();

  private void Awake() {
    // Spawn a display for each enemy in the evaluator
    if (!evaluator) {
      Debug.LogError("Evaluator is not assigned in WristData.");
      return;
    }
    var enemyCount = evaluator.GetSpawnManager().GetTotalEnemies();
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
    // foreach (var hit in evaluator.hits) {
    //   var enemyDisplay = enemyDisplays.Find(ed => ed.name == "EnemyDisplay_" + hit.Key);
    //   if (enemyDisplay) {
    //     enemyDisplay.DisplayDamagedPart(hit.Value);
    //   } else {
    //     // Debug.LogWarning($"No display found for enemy {hit.Key}");
    //   }
    // }
    var keys = evaluator.hits.Keys;
    for (var i = 0; i < evaluator.hits.Count; i++) {
      var enemyDisplay = enemyDisplays[i];
      if (enemyDisplay) {
        var hit = evaluator.hits[keys.ElementAt(i)];
        enemyDisplay.DisplayDamagedPart(hit);
      } else {
        Debug.LogWarning($"No display found for enemy {i}");
      }
    }
  }
}
