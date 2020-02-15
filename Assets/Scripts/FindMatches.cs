using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour {
    private Board board;

    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    private void Start() {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches() {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine() {
        yield return new WaitForSeconds(.2f);
        for (var i = 0; i < board.width; i++) {
            for (var j = 0; j < board.height; j++) {
                var currentDot = board.allDots[i, j];
                if (currentDot != null) {
                    if (i > 0 && i < board.width - 1) {
                        var leftDot = board.allDots[i - 1, j];
                        var rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null) {
                            if (leftDot.CompareTag(currentDot.tag) && rightDot.CompareTag(currentDot.tag)) {
                                leftDot.GetComponent<Dot>().isMatched = true;
                                rightDot.GetComponent<Dot>().isMatched = true;
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1) {
                        var upDot = board.allDots[i, j + 1];
                        var downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null) {
                            if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag)) {
                                upDot.GetComponent<Dot>().isMatched = true;
                                downDot.GetComponent<Dot>().isMatched = true;
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}