using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board _board;
    private Piece _piece;
    private int _clears, _level, _highScore, _score; 
    
    [Header("UI")] 
    [SerializeField] private Text _clearsText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _highScoreText;
    [SerializeField] private Text _scoreText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _board = GetComponent<Board>();
        _piece = GetComponent<Piece>();
        
        _board.OnClear += BoardOnClear;
        _piece.OnGameOver += OnGameOver;
        
        _clears = 0;
        _level = 1;
        _score = 0;
        _highScore = PlayerPrefsManager.GetHighScore();
        
        UpdateClearsUI();
        UpdateScoreUI();
        UpdateLevelUI();
        UpdateHighScoreUI();
    }

    private void OnGameOver()
    {
        if (_score > _highScore)
        {
            _highScore = _score;
            UpdateHighScoreUI();
            PlayerPrefsManager.SaveHighScore(_highScore);
        }
    }

    private void BoardOnClear()
    {
        _clears++;
        _score += 100;
        
        if(_clears % 10 == 0) _piece.IncreaseStepDelay(-.1f);
        
        UpdateClearsUI();
        UpdateScoreUI();
    }

    private void UpdateClearsUI()
    {
        _clearsText.text = "Clears: \n" + _clears;
    }
    
    private void UpdateLevelUI()
    {
        _levelText.text = "Level: \n" + _level;
    }
    
    private void UpdateScoreUI()
    {
        _scoreText.text = "Score: \n" + _score;
    }
    
    private void UpdateHighScoreUI()
    {
        _highScoreText.text = "High Score: \n" + _highScore;
    }
}
