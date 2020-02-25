using System;
using Entity;
using Enum;
using UnityEngine;
using UnityEngine.UI;

namespace Managers {
    public class EndGameManager : MonoBehaviour {
        public EndGameRequirementState requirementState;
        public GameObject movesLabel;
        public GameObject timeLabel;
        public Text counter;
        private int currentCounterValue;
        private float timerSeconds;
        public GameObject casanova;
        public AudioSource casanovaSource;
        private Board board;
        public GameObject winPanel;
        public GameObject losePanel;

        // Start is called before the first frame update
        private void Start() {
            board = FindObjectOfType<Board>();
            SetUpGameType();
            SetUpGame();
        }

        private void SetUpGameType() {
            requirementState = board.world.levels[board.level].endGameRequirementState;
        }

        private void SetUpGame() {
            currentCounterValue = requirementState.remainingValue;
            switch (requirementState.levelLimitationType) {
                case LevelLimitationType.MOVES:
                    movesLabel.SetActive(true);
                    timeLabel.SetActive(false);
                    break;
                case LevelLimitationType.TIME:
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
            ResetEndGameRequirementsCountdownState();
        }

        private void LoseGame() {
            board.currentState = GameState.LOSE;
            losePanel.SetActive(true);
            ResetEndGameRequirementsCountdownState();
            /*casanova.SetActive(true);
        casanovaSource.Play();*/
        }

        private void ResetEndGameRequirementsCountdownState() {
            currentCounterValue = 0;
            counter.text = "" + currentCounterValue;
        }

        // Update is called once per frame
        private void Update() {
            if (requirementState.levelLimitationType == LevelLimitationType.TIME && currentCounterValue > 0) {
                timerSeconds -= Time.deltaTime;
                if (timerSeconds <= 0) {
                    DecreaseCounterValue();
                    timerSeconds = 1;
                }
            }
        }
    }
}