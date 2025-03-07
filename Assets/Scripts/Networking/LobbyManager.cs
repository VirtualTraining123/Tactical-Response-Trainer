using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    
    [SerializeField] private NetworkRunner networkRunner;

    [SerializeField] private int evaluationSceneBuildIndex = 1;
   
    public void StartEvaluationScene()
    {
        if (networkRunner == null)
        {
            Debug.LogError("NetworkRunner no asignado en el Inspector.");
            return;
        }

        if (!networkRunner.IsSceneAuthority)
        {
            Debug.LogWarning("Solo el host puede cambiar de escena.");
            return;
        }

        SceneRef evaluationScene = SceneRef.FromIndex(evaluationSceneBuildIndex);

        networkRunner.LoadScene(evaluationScene, LoadSceneMode.Additive);
    }
}