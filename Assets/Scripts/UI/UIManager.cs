using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Menus")]
    [SerializeField] GameObject BeforeStartGame;
    [SerializeField] GameObject InGameMenu;
    [SerializeField] GameObject RestartGameMenu;
    [SerializeField] GameObject PauseMenu;

    [Header("Texts")]
    [SerializeField] GameObject YouWinText;
    [SerializeField] GameObject YouLoseText;
    [SerializeField] TMP_Text Score;
    public TMP_Text RemainigTime;

    public Enums.GameStates gameStates;

    protected override void Awake()
    {
        base.Awake();
        BeforeStartGame.SetActive(true);
        InGameMenu.SetActive(false);
        RestartGameMenu.SetActive(false);
        PauseMenu.SetActive(false);
        gameStates = Enums.GameStates.BeforeStartMenuisOpen;
    }

    private void Update()
    {
        Score.text = "Score: " + Player.Instance.currentScore.ToString();
        RemainigTime.text = "Time: " + GameManager.Instance.time.ToString();



        switch (gameStates)
        {
            case Enums.GameStates.BeforeStartMenuisOpen:
                Time.timeScale = 0;
                break;
            case Enums.GameStates.GameStarted:
                Time.timeScale = 1;
                break;
            case Enums.GameStates.GamePaused:
                Time.timeScale = 0;
                break;
            case Enums.GameStates.GameResumed:
                Time.timeScale = 1;
                break;
            case Enums.GameStates.GameFinished:
                GameFinishEvents();
                break;
        }
    }

    //If the game is finished check if the player is dead or time runed out or All the enemies are dead by that function Give you win or you lose text
    private void GameFinishEvents()
    {
        Time.timeScale = 0;
        InGameMenu.SetActive(false);
        PauseMenu.SetActive(false);
        RestartGameMenu.SetActive(true);

        if (Player.Instance.isPlayerDeath == true)
        {
            YouLoseText.SetActive(true);
        }
        else if (GameManager.Instance.AllTheEnemiesAreDead == true || GameManager.Instance.TimeRunOut == true)
        {
            YouWinText.SetActive(true);
        }
    }

    #region Button Behaviour
    public void StartGameButton()
    {
        BeforeStartGame.SetActive(false);
        InGameMenu.SetActive(true);
        gameStates = Enums.GameStates.GameStarted;
    }

    public void PauseButtonClicked()
    {
        gameStates = Enums.GameStates.GamePaused;
        PauseMenu.SetActive(true);
    }

    public void ResumeButtonClicked()
    {
        gameStates = Enums.GameStates.GameResumed;       
        PauseMenu.SetActive(false);   
    }

    public void RestartButtonClicked()
    {
        BeforeStartGame.SetActive(true);
        SceneManager.LoadScene("SampleScene");
        Player.Instance.isPlayerDeath = false;
    }
    #endregion
}
