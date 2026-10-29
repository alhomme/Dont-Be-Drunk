using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public SaveScore pGoldScore;
    public SaveScore pSilverScore;
    public SaveScore pBronzeScore;
    public bool pTutorialSeen;

    public SaveData()
    {
        pGoldScore = new SaveScore();
        pSilverScore = new SaveScore();
        pBronzeScore = new SaveScore();
        pTutorialSeen = false;
    }
}

[Serializable]
public class SaveScore
{
    public string pName;
    public int pScore;
}