using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_SceneLoaded mRSE_SceneLoaded;

    [Header("Fade")]
    [SerializeField] private CanvasGroup mFadeCanvas;


    private string mActiveScene;

    private void OnEnable()
    {
        MyUtils.Log("SceneLoadManager.OnEnable: Active scene = " + SceneManager.GetActiveScene().name);

        mActiveScene = SceneManager.GetActiveScene().name;
        mRSE_LoadScene.Event += OnLoadScene;
        SceneManager.sceneLoaded += OnSceneLoaded;

        mFadeCanvas.alpha = 0f;
    }

    private void OnDisable()
    {
        mRSE_LoadScene.Event -= OnLoadScene;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        mFadeCanvas.alpha = 0f;
    }

    private void OnLoadScene(string scene)
    {
        FadeToBlack(scene);
    }

    private void FadeToBlack(string scene)
    {
        LMotion.Create(0f, 1f, 0.5f)
            .WithOnComplete(() => UnloadAndLoad(scene))
            .BindToAlpha(mFadeCanvas);
    }

    private void UnloadAndLoad(string scene)
    {
        MyUtils.Log("SceneLoader: Unload " + mActiveScene);
        SceneManager.UnloadSceneAsync(mActiveScene);

        MyUtils.Log("SceneLoader: Load: " + scene);
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TechnicalScene")
            return;

        FadeFromBlack();

        MyUtils.Log("SceneLoader: OnSceneLoaded: " + scene.name);
        SceneManager.SetActiveScene(scene);
        mActiveScene = scene.name;

        mRSE_SceneLoaded.Dispatch(scene.name);
    }

    private void FadeFromBlack()
    {
        LMotion.Create(1f, 0f, 0.5f)
            .BindToAlpha(mFadeCanvas);
    }
}
