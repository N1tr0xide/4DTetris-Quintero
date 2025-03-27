using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _howToPanel;

    public void Start()
    {
        SetHowToPanelActive(false);
        FindFirstObjectByType<Piece>().IncreaseStepDelay(-.5f);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetHowToPanelActive(bool activeState)
    {
        _howToPanel.SetActive(activeState);
    }
}
