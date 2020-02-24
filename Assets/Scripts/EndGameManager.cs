using System;
using UnityEngine;
using UnityEngine.UI;

public enum GameType {
    MOVES,
    TIME
}

[System.Serializable]
public class EndGameRequirements {
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour {
    public EndGameRequirements requirements;
    public GameObject movesLabel;
    public GameObject timeLabel;
    public Text counter;
    public int currentCounterValue;
    private float timerSeconds;
    public GameObject casanova;
    public AudioSource casanovaSource;
    private Board board;
    public GameObject winPanel;
    public GameObject losePanel;

    // Start is called before the first frame update
    private void Start() {
        board = FindObjectOfType<Board>();
        SetUpGame();
    }

    private void SetUpGame() {
        currentCounterValue = requirements.counterValue;
        switch (requirements.gameType) {
            case GameType.MOVES:
                movesLabel.SetActive(true);
                timeLabel.SetActive(false);
                break;
            case GameType.TIME:
                timerSeconds = 1;
                movesLabel.SetActive(false);
                timeLabel.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue() {
        if (board.currentState == GameState.MOVE) {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0) {
                LoseGame();
            }
        }
    }

    public void WinGame() {
        winPanel.SetActive(true);
        board.currentState = GameState.WIN;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
    }

    public void LoseGame() {
        board.currentState = GameState.LOSE;
        losePanel.SetActive(true);
        /*casanova.SetActive(true);
        casanovaSource.Play();*/
        
    }

    // Update is called once per frame
    private void Update() {
        //todo начинать отсчет только после старта
        if (requirements.gameType == GameType.TIME && currentCounterValue > 0) {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0) {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}