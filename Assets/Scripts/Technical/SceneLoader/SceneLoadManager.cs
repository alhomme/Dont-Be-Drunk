using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_SceneLoaded mRSE_SceneLoaded;


    private string mActiveScene;

    private void OnEnable()
    {
        MyUtils.Log("SceneLoadManager.OnEnable");

        mActiveScene = "StarterScene";
        mRSE_LoadScene.Event += OnLoadScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        mRSE_LoadScene.Event -= OnLoadScene;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnLoadScene(string scene)
    {
        MyUtils.Log("SceneLoader: Unload " + mActiveScene);
        SceneManager.UnloadSceneAsync(mActiveScene);

        MyUtils.Log("SceneLoader: Load: " + scene);
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        mActiveScene = scene;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MyUtils.Log("SceneLoader: OnSceneLoaded: " + scene.name);
        SceneManager.SetActiveScene(scene);

        mRSE_SceneLoaded.Dispatch(scene.name);
    }
}
