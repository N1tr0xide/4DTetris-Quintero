using UnityEngine;

public static class PlayerPrefsManager
{
    private const string HighScore = "HighScore";

    public static int GetHighScore()
    {
        return PlayerPrefs.HasKey(HighScore)? PlayerPrefs.GetInt(HighScore) : 0;
    }

    public static void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt(HighScore, score);
        PlayerPrefs.Save();
    }
}