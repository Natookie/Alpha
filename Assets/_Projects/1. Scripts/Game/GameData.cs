using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    [Header("SESSION")]
    public int startingHealth = 3;
    public int pointsPerCorrectGuess = 1;
    public int damagePerWrongGuess = 1;

    [Header("CARD RULES")]
    public bool aceHigh = true;
    public bool breakTiesBySuit = true;
    public TieResult tieResult = TieResult.Loss;

    [Header("PRESENTATION")]
    public float gameOverDelay = 2f;
    public int cardShowCount = 5;
}

public enum TieResult { Loss, Push, Win }