using UnityEngine;
using UnityEngine.SceneManagement;

public class StarterManager : MonoBehaviour
{
    [SerializeField] RSE_LoadScene mRSE_LoadScene;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        MyUtils.Log("StarterManager");
        SceneManager.LoadSceneAsync("TechnicalScene", LoadSceneMode.Additive);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MyUtils.Log("StarterManager: OnSceneLoaded: " + scene.name);
        mRSE_LoadScene.Dispatch("GameScene");
    }
}
