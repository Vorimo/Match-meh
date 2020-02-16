using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
                                if (currentDot.GetComponent<Dot>().isRowBomb || leftDot.GetComponent<Dot>().isRowBomb
                                                                             || rightDot.GetComponent<Dot>()
                                                                                 .isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (currentDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (leftDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }

                                if (rightDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }

                                if (!currentMatches.Contains(leftDot)) {
                                    currentMatches.Add(leftDot);
                                }

                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(rightDot)) {
                                    currentMatches.Add(rightDot);
                                }

                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                }

                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1) {
                        var upDot = board.allDots[i, j + 1];
                        var downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null) {
                            if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag)) {
                                if (currentDot.GetComponent<Dot>().isColumnBomb || upDot.GetComponent<Dot>()
                                                                                    .isColumnBomb
                                                                                || downDot.GetComponent<Dot>()
                                                                                    .isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (currentDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (upDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                }

                                if (downDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                }

                                if (!currentMatches.Contains(upDot)) {
                                    currentMatches.Add(upDot);
                                }

                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(downDot)) {
                                    currentMatches.Add(downDot);
                                }

                                downDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                }

                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column) {
        List<GameObject> dots = new List<GameObject>();
        for (var i = 0; i < board.height; i++) {
            if (board.allDots[column, i] != null) {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (var i = 0; i < board.width; i++) {
            if (board.allDots[i, row] != null) {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs() {
        //the player move something
        if (board.currentDot != null) {
            //moved piece matched
            if (board.currentDot.isMatched) {
                //make it unmatched
                board.currentDot.isMatched = false;
                //decide which bomb type to make
                if ((board.currentDot.swipeAngle > 45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 && board.currentDot.swipeAngle >= 135)) {
                    board.currentDot.MakeRowBomb();
                }
                else {
                    board.currentDot.MakeColumnBomb();
                }
            }
            //the other piece matched
            else if (board.currentDot.nextDot != null) {
                var otherDot = board.currentDot.nextDot.GetComponent<Dot>();
                //the other dot matched
                if (otherDot.isMatched) {
                    otherDot.isMatched = false;
                    //decide which bomb type to make
                    if ((board.currentDot.swipeAngle > 45 && board.currentDot.swipeAngle <= 45)
                        || (board.currentDot.swipeAngle < -135 && board.currentDot.swipeAngle >= 135)) {
                        otherDot.MakeRowBomb();
                    }
                    else {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}