using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_SaveData mRSO_SaveData;

    [Header("RSE")]
    [SerializeField] private RSE_Save mRSE_Save;

    private string mSaveFilename = "/save.json";

    private void OnEnable()
    {
        mRSE_Save.Event += OnSave;

        if (!DoesSaveFileExits())
            CreateSaveFile();
        else
            ReadSaveFile();
    }

    private void OnDisable()
    {
        mRSE_Save.Event -= OnSave;
    }

    private void OnSave(SaveData saveData)
    {
        mRSO_SaveData.Value = saveData;
        WriteSaveFile();
    }

    private bool DoesSaveFileExits()
    {
        string path = Application.persistentDataPath + mSaveFilename;
        return File.Exists(path);
    }

    private void CreateSaveFile()
    {
        mRSO_SaveData.Value.pGoldScore = new SaveScore
        {
            pName = "",
            pScore = 0
        };

        mRSO_SaveData.Value.pSilverScore = new SaveScore
        {
            pName = "",
            pScore = 0
        };

        mRSO_SaveData.Value.pBronzeScore = new SaveScore
        {
            pName = "",
            pScore = 0
        };

        mRSO_SaveData.Value.pTutorialSeen = false;

        WriteSaveFile();
    }

    private void ReadSaveFile()
    {
        string path = Application.persistentDataPath + mSaveFilename;
        string rawContent = File.ReadAllText(path);

        mRSO_SaveData.Value = JsonUtility.FromJson<SaveData>(rawContent);
    }

    private void WriteSaveFile()
    {
        string json = JsonUtility.ToJson(mRSO_SaveData.Value);
        string path = Application.persistentDataPath + mSaveFilename;

        File.WriteAllText(path, json);
    }
}
