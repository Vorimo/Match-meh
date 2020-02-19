using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    private Board board;
    public Text scoreText;
    public int score;
    public Image scoreBar;

    private void Start() {
        board = FindObjectOfType<Board>();
        UpdateBar();
    }

    // Update is called once per frame
    private void Update() {
        scoreText.text = "" + score;
    }

    public void IncreaseScore(int amountToIncrease) {
        score += amountToIncrease;
        UpdateBar();
    }

    private void UpdateBar() {
        if (board != null && scoreBar != null) {
            var length = board.scoreGoals.Length;
            // cast for correct arithmetic operation
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}