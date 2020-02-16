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

    private void ProcessAdjacentBombAffectedElementsIfExists(IEnumerable<Dot> dots) {
        foreach (var dot in dots.Where(dot => dot.isAdjacentBomb)) {
            currentMatches.Union(GetAdjacentPieces(dot.column, dot.row));
        }
    }

    private void ProcessRowBombAffectedElementsIfBombExists(IEnumerable<Dot> dots) {
        foreach (var dot in dots.Where(dot => dot.isRowBomb)) {
            currentMatches.Union(GetRowPieces(dot.row));
        }
    }

    private void ProcessColumnBombAffectedElementsIfBombExists(IEnumerable<Dot> dots) {
        foreach (var dot in dots.Where(dot => dot.isColumnBomb)) {
            currentMatches.Union(GetColumnPieces(dot.column));
        }
    }

    private void ProcessElementsAffectedByBombIfBombExists(List<Dot> dots) {
        ProcessRowBombAffectedElementsIfBombExists(dots);
        ProcessColumnBombAffectedElementsIfBombExists(dots);
    }

    private void ProcessNearbyPieces(List<GameObject> dots) {
        foreach (var dot in dots) {
            if (!currentMatches.Contains(dot)) {
                currentMatches.Add(dot);
            }

            dot.GetComponent<Dot>().isMatched = true;
        }
    }

    private IEnumerator FindAllMatchesCoroutine() {
        yield return new WaitForSeconds(.2f);
        for (var i = 0; i < board.width; i++) {
            for (var j = 0; j < board.height; j++) {
                var currentDot = board.allDots[i, j];
                if (currentDot != null) {
                    var currentDotComponent = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1) {
                        var leftDot = board.allDots[i - 1, j];
                        var rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null) {
                            var leftDotComponent = leftDot.GetComponent<Dot>();
                            var rightDotComponent = rightDot.GetComponent<Dot>();
                            if (leftDot.CompareTag(currentDot.tag) && rightDot.CompareTag(currentDot.tag)) {
                                ProcessElementsAffectedByBombIfBombExists(new List<Dot>
                                    {currentDotComponent, leftDotComponent, rightDotComponent});

                                ProcessAdjacentBombAffectedElementsIfExists(new List<Dot>
                                    {currentDotComponent, leftDotComponent, rightDotComponent});

                                ProcessNearbyPieces(new List<GameObject> {currentDot, leftDot, rightDot});
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1) {
                        var upDot = board.allDots[i, j + 1];
                        var downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null) {
                            var upDotComponent = upDot.GetComponent<Dot>();
                            var downDotComponent = downDot.GetComponent<Dot>();
                            if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag)) {
                                ProcessElementsAffectedByBombIfBombExists(new List<Dot>
                                    {currentDotComponent, downDotComponent, upDotComponent});
                                ProcessAdjacentBombAffectedElementsIfExists(new List<Dot>
                                    {currentDotComponent, downDotComponent, upDotComponent});

                                ProcessNearbyPieces(new List<GameObject> {currentDot, downDot, upDot});
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color) {
        for (var i = 0; i < board.width; i++) {
            for (var j = 0; j < board.height; j++) {
                //if piece exists
                if (board.allDots[i, j] != null) {
                    if (board.allDots[i, j].CompareTag(color)) {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    private List<GameObject> GetColumnPieces(int column) {
        var dots = new List<GameObject>();
        for (var i = 0; i < board.height; i++) {
            if (board.allDots[column, i] != null) {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    private List<GameObject> GetAdjacentPieces(int column, int row) {
        var dots = new List<GameObject>();
        for (var i = column - 1; i <= column + 1; i++) {
            for (var j = row - 1; j <= row + 1; j++) {
                //the piece is inside the board
                if (i >= 0 && i < board.width && j >= 0 && j < board.height) {
                    dots.Add(board.allDots[i, j]);
                    board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }
            }
        }

        return dots;
    }

    private List<GameObject> GetRowPieces(int row) {
        var dots = new List<GameObject>();
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