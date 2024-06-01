using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<EnemySpawner> enemySpawners;
    [SerializeField] private List<CivilSpawner> civilSpawners;

    private int totalEnemies= 0;
    private int totalCivilians= 0;

    private void Start()
    {
        CalculateTotalEntities();
    }

    private void CalculateTotalEntities()
    {


        foreach (EnemySpawner spawner in enemySpawners)
        {
            totalEnemies += spawner.GetMaxEnemiesNumber();
        }

        foreach (CivilSpawner spawner in civilSpawners)
        {
            totalCivilians += spawner.GetMaxCivilNumber();
        }

        // Example of logging the totals
        Debug.Log("Total Enemies: " + totalEnemies);
        Debug.Log("Total Civilians: " + totalCivilians);
    }

    public int GetTotalEnemies() => totalEnemies;
    public int GetTotalCivilians() => totalCivilians;

   
}
