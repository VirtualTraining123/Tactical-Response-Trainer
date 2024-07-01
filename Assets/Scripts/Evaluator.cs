using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Evaluator : MonoBehaviour
{
    [Header("Settings")]
    public float maxSimulationTime = 90f; // 1 minute and 30 seconds
    public float maxScore = 10f;
    public float minPassingScore = 6f;
    public float scorePenaltyPerEnemy = 1f;
    public float scorePenaltyForCivilian = 2f;
    public float scorePenaltyForExtraBullet = 0.25f;
    public float scorePenaltyForUnloading = 1f;

    private float currentScore;
    private float elapsedTime;
    private bool simulationEnded = false;
    private bool PlayerDead = false;
    private String PlayerDeadString="No";
    public String SeguroAlFinal="Si";

    private int enemytoneutralize=0;
    private int civilianhit=0;
    private int totalEnemys;
     private int bullettouse;
     private int bulletused;
     private SpawnManager spawnManager;
     Pistol pistol;

     private char condition;
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        
        bullettouse=spawnManager.GetTotalEnemies();
        
        
            // Restablecer variables al inicio de la simulación
        elapsedTime = 0f;
        pistol = FindObjectOfType<Pistol>();

        totalEnemys=0;
        enemytoneutralize=0;
        bulletused=0;
        
        PlayerDeadString="No";
        civilianhit=0;
        currentScore=10;

    }

    void RestScore(float score){
         currentScore -= score;
    currentScore = Mathf.Clamp(currentScore, 1f, 10f); // Restringir la puntuación dentro del rango de 1 a 10
    }

    void Update()
    {
        if (simulationEnded)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= maxSimulationTime)
        {
           
            EndSimulation();
        }
    }
    public void EnemyNeutralized()
    {
        enemytoneutralize--;
    }
    public void EnemyMissed()
    {
        RestScore(scorePenaltyPerEnemy*(totalEnemys+enemytoneutralize));
    }

    public void CivilianHit()
    {
        civilianhit++;
        RestScore(scorePenaltyForCivilian);
        Debug.Log("Tota civil impacted: " + civilianhit);
    }

    public void BulletUsed()
    {
         bulletused++;
    }

    public void TotalpointBulletUsed()
    {
        int total=bulletused-bullettouse;
        if(total>0){
            RestScore(scorePenaltyForExtraBullet*total);
        }else{
            RestScore(0);
        }
       
    }

    public void CheckUnloadPenalty()
    {
       
        if (pistol.isSafetyOn != true)
        {
            UnloadPenalty();
        }
    }

    public void UnloadPenalty()
    {
        SeguroAlFinal="No";
        RestScore(scorePenaltyForUnloading);
        
    }
    
    public void ReceiveShot()
    {   PlayerDead=true;
        PlayerDeadString="Si";
        EndSimulation();
    }

    public void EarlyEndSimulation(){
        Invoke("EndSimulation", 5f);
    }
    private void EndSimulation()
    {   totalEnemys=spawnManager.GetTotalEnemies();
        Debug.Log("Total Enemies to Spawn: " + spawnManager.GetTotalEnemies());
        Debug.Log("Total Civilians to Spawn: " + spawnManager.GetTotalCivilians());
        EnemyMissed();
        CheckUnloadPenalty();
        TotalpointBulletUsed();
        

        simulationEnded = true;
        PlayerPrefs.SetFloat("Tiempo", elapsedTime);
        PlayerPrefs.SetInt("Civiles_Heridos",civilianhit);
        PlayerPrefs.SetInt("Enemigos_Faltantes", totalEnemys + enemytoneutralize);
        PlayerPrefs.SetInt("Cartuchos_extra_gastados", bulletused - bullettouse);
        PlayerPrefs.SetFloat("Puntaje_Final", currentScore);
        PlayerPrefs.SetString("Muerte_de_agente", PlayerDeadString);
        PlayerPrefs.SetString("Seguro", SeguroAlFinal);
       
        
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;
        //SaveDataToCSV();

        //realizamos una saturacion de currentScore para que no sea negativo
      

        if (currentScore < minPassingScore || PlayerDead==true)
        {
            condition='D';
            

           SceneTransitionManager.singleton.GoToSceneAsync(3);
        }
        else if (currentScore >= minPassingScore && currentScore < maxScore && PlayerDead==false)
        {
            condition='A';
            

           SceneTransitionManager.singleton.GoToSceneAsync(4);
        }
       

    }

    public float GetCurrentScore()
    {
        return currentScore;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public int GetCiviliansInjured()
    {
        return civilianhit;
    }

    public int GetEnemiesEliminated()
    {
        return -enemytoneutralize;
    }

    public bool IsSimulationEnded()
    {
        return simulationEnded;
    }

    public String GetSafetyLock()
    {
        if(pistol.isSafetyOn){
            return "Si";
        }else{
            return "No";
        }
        
    }

    public int GetBullets()
    {
        // se verifican las balas actuales del cartucho de pistol
        return pistol.GetBullets();
    }
/*
    private void OnGUI()
    {
        GUI.Label(new Rect(40, 50, 200, 20), "Amenazas restantes: " + enemytoneutralize);
        GUI.Label(new Rect(450, 20, 300, 20), "Tiempo transcurrido: " + elapsedTime);
        GUI.Label(new Rect(40, 90, 200, 20), "Disparos realizados: " + bulletused);
        GUI.Label(new Rect(40, 110, 200, 20), "Puntaje actual: " + currentScore);
    }
    */
/*
    private void SaveDataToCSV()
{
    // Crear el nombre del archivo con la fecha y hora actual
    string fileName = "evaluator_data_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
    string filePath;

    // Verificar si se está ejecutando en un Oculus Quest 2
   /* if (IsOculusQuest2())
    {
        // Si es un Oculus Quest 2, guardar en la carpeta Documents
        filePath = Path.Combine("/storage/emulated/0/Documents", fileName);
    }
    else
    {
        // De lo contrario, guardar en la carpeta StreamingAssets
        filePath = Path.Combine(Application.streamingAssetsPath, fileName);
    //}

    // Crear el archivo CSV y escribir los datos
    using (StreamWriter writer = new StreamWriter(filePath))
    {
        // Escribir los encabezados de las columnas
        writer.WriteLine("Tiempo de simulacion;Enemigos Faltantes;Civiles impactados;Exceso de balas;Seguro al final del escenario;Muerte del agente;Puntaje Final;Condicion Aprobado o Desaprobado");

        // Escribir los datos de las variables
        writer.WriteLine($"{elapsedTime};{totalEnemys + enemytoneutralize};{civilianhit};{bulletused - bullettouse};{pistol.isSafetyOn};{PlayerDead};{currentScore};{condition}");
    }

    Debug.Log("Datos guardados en: " + filePath);
}
*/
// Método para verificar si se está ejecutando en un Oculus Quest 2
/*
private bool IsOculusQuest2()
{
    // Verificar si el dispositivo actual es de la familia Oculus Quest
    if (XRDevice
    {
        // Verificar si es un Oculus Quest 2 basado en su nombre de modelo
        if (XRDevice.model.Contains("Quest 2"))
        {
            return true;
        }
    }
    
    return false;
}*/

}
