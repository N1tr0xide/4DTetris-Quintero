using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Text _clearsText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _highScoreText;
    [SerializeField] private Text _scoreText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameOverUI(false);
    }

    public void SetGameOverUI(bool activeState)
    {
        _gameOverPanel.SetActive(activeState);
    }
    
    public void UpdateClearsUI(int clears) 
    {
        _clearsText.text = "Clears: \n" + clears;
    }
    
    public void UpdateLevelUI(int level) 
    {
        _levelText.text = "Level: \n" + level;
    }
    
    public void UpdateScoreUI(int score) 
    {
        _scoreText.text = "Score: \n" + score;
    }
    
    public void UpdateHighScoreUI(int highScore) 
    {
        _highScoreText.text = "High Score: \n" + highScore;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
