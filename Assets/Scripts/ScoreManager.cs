using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board _board;
    private Piece _piece;
    private int _clears, _level, _highScore, _score;
    
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private AudioClip _clearSound;

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
        
        _gameUIManager.UpdateClearsUI(_clears);
        _gameUIManager.UpdateScoreUI(_score);
        _gameUIManager.UpdateLevelUI(_level);
        _gameUIManager.UpdateHighScoreUI(_highScore);
    }

    private void OnGameOver()
    {
        _gameUIManager.SetGameOverUI(true);

        if (_score <= _highScore) return;
        _highScore = _score;
        _gameUIManager.UpdateHighScoreUI(_highScore);
        PlayerPrefsManager.SaveHighScore(_highScore);
    }

    private void BoardOnClear()
    {
        AudioManager.Instance.PlayOneShot(_clearSound);
        _clears++;
        _score += 100;
        
        if(_clears % 10 == 0) // every 10 clears
        {
            _piece.IncreaseStepDelay(-.1f);
            _level++;
            _gameUIManager.UpdateLevelUI(_level);
        }
        
        _gameUIManager.UpdateClearsUI(_clears);
        _gameUIManager.UpdateScoreUI(_score);
    }
}
