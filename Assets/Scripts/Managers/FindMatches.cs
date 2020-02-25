using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity;
using UnityEngine;

namespace Managers {
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

        private void ProcessAdjacentBombAffectedElementsIfExists(IEnumerable<Element> dots) {
            foreach (var dot in dots.Where(dot => dot.isAdjacentBomb)) {
                currentMatches.Union(GetAdjacentPieces(dot.column, dot.row));
            }
        }

        private void ProcessRowBombAffectedElementsIfBombExists(IEnumerable<Element> dots) {
            foreach (var dot in dots.Where(dot => dot.isRowBomb)) {
                currentMatches.Union(GetRowPieces(dot.row));
            }
        }

        private void ProcessColumnBombAffectedElementsIfBombExists(IEnumerable<Element> dots) {
            foreach (var dot in dots.Where(dot => dot.isColumnBomb)) {
                currentMatches.Union(GetColumnPieces(dot.column));
            }
        }

        private void ProcessElementsAffectedByBombIfBombExists(List<Element> dots) {
            ProcessRowBombAffectedElementsIfBombExists(dots);
            ProcessColumnBombAffectedElementsIfBombExists(dots);
        }

        private void ProcessNearbyPieces(List<GameObject> dots) {
            foreach (var dot in dots) {
                if (!currentMatches.Contains(dot)) {
                    currentMatches.Add(dot);
                }

                dot.GetComponent<Element>().isMatched = true;
            }
        }

        private IEnumerator FindAllMatchesCoroutine() {
            //todo убрать задержки
            yield return new WaitForSeconds(.2f);
            for (var i = 0; i < board.width; i++) {
                for (var j = 0; j < board.height; j++) {
                    var currentDot = board.allDots[i, j];
                    if (currentDot != null) {
                        var currentDotComponent = currentDot.GetComponent<Element>();
                        if (i > 0 && i < board.width - 1) {
                            var leftDot = board.allDots[i - 1, j];
                            var rightDot = board.allDots[i + 1, j];
                            if (leftDot != null && rightDot != null) {
                                var leftDotComponent = leftDot.GetComponent<Element>();
                                var rightDotComponent = rightDot.GetComponent<Element>();
                                if (leftDot.CompareTag(currentDot.tag) && rightDot.CompareTag(currentDot.tag)) {
                                    ProcessElementsAffectedByBombIfBombExists(new List<Element> {
                                        currentDotComponent, leftDotComponent, rightDotComponent
                                    });

                                    ProcessAdjacentBombAffectedElementsIfExists(new List<Element> {
                                        currentDotComponent, leftDotComponent, rightDotComponent
                                    });

                                    ProcessNearbyPieces(new List<GameObject> {currentDot, leftDot, rightDot});
                                }
                            }
                        }

                        if (j > 0 && j < board.height - 1) {
                            var upDot = board.allDots[i, j + 1];
                            var downDot = board.allDots[i, j - 1];
                            if (upDot != null && downDot != null) {
                                var upDotComponent = upDot.GetComponent<Element>();
                                var downDotComponent = downDot.GetComponent<Element>();
                                if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag)) {
                                    ProcessElementsAffectedByBombIfBombExists(new List<Element> {
                                        currentDotComponent, downDotComponent, upDotComponent
                                    });
                                    ProcessAdjacentBombAffectedElementsIfExists(new List<Element> {
                                        currentDotComponent, downDotComponent, upDotComponent
                                    });

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
                            board.allDots[i, j].GetComponent<Element>().isMatched = true;
                        }
                    }
                }
            }
        }

        private IEnumerable<GameObject> GetColumnPieces(int column) {
            var dots = new List<GameObject>();
            for (var i = 0; i < board.height; i++) {
                if (board.allDots[column, i] != null) {
                    var dot = board.allDots[column, i].GetComponent<Element>();
                    if (dot.isRowBomb) {
                        dots.Union(GetRowPieces(i));
                    }

                    dots.Add(board.allDots[column, i]);
                    dot.isMatched = true;
                }
            }

            return dots;
        }

        private IEnumerable<GameObject> GetAdjacentPieces(int column, int row) {
            var dots = new List<GameObject>();
            for (var i = column - 1; i <= column + 1; i++) {
                for (var j = row - 1; j <= row + 1; j++) {
                    //the piece is inside the board
                    if (i >= 0 && i < board.width && j >= 0 && j < board.height) {
                        if (board.allDots[i, j] != null) {
                            dots.Add(board.allDots[i, j]);
                            board.allDots[i, j].GetComponent<Element>().isMatched = true;
                        }
                    }
                }
            }

            return dots;
        }

        private IEnumerable<GameObject> GetRowPieces(int row) {
            var dots = new List<GameObject>();
            for (var i = 0; i < board.width; i++) {
                if (board.allDots[i, row] != null) {
                    var dot = board.allDots[i, row].GetComponent<Element>();
                    if (dot.isColumnBomb) {
                        dots.Union(GetColumnPieces(i)).ToList();
                    }

                    dots.Add(board.allDots[i, row]);
                    dot.isMatched = true;
                }
            }

            return dots;
        }

        public void CheckBombs() {
            //the player move something
            if (board.currentElement != null) {
                //moved piece matched
                if (board.currentElement.isMatched) {
                    //make it unmatched
                    board.currentElement.isMatched = false;
                    //decide which bomb type to make
                    if ((board.currentElement.swipeAngle > 45 && board.currentElement.swipeAngle <= 45)
                        || (board.currentElement.swipeAngle < -135 && board.currentElement.swipeAngle >= 135)) {
                        board.currentElement.MakeRowBomb();
                    } else {
                        board.currentElement.MakeColumnBomb();
                    }
                }
                //the other piece matched
                else if (board.currentElement.nextDot != null) {
                    var otherDot = board.currentElement.nextDot.GetComponent<Element>();
                    //the other dot matched
                    if (otherDot.isMatched) {
                        otherDot.isMatched = false;
                        //decide which bomb type to make
                        if ((board.currentElement.swipeAngle > 45 && board.currentElement.swipeAngle <= 45)
                            || (board.currentElement.swipeAngle < -135 && board.currentElement.swipeAngle >= 135)) {
                            otherDot.MakeRowBomb();
                        } else {
                            otherDot.MakeColumnBomb();
                        }
                    }
                }
            }
        }
    }
}