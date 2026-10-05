using UnityEngine;

[CreateAssetMenu(fileName = "RSE_GameOver", menuName = "RSE/RSE Game Over")]
public class RSE_GameOver : RuntimeScriptableEvent<GameOverType>
{
}

public enum GameOverType
{
    Life,
    Alcohol,
    Police
}
