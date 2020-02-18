using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public Text scoreText;

    public int score;

    // Update is called once per frame
    private void Update() {
        scoreText.text = "" + score;
    }

    public void IncreaseScore(int amountToIncrease) {
        score += amountToIncrease;
    }
}