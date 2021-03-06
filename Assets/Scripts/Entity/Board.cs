﻿using System.Collections;
using System.Collections.Generic;
using Enum;
using Managers;
using Scriptable_Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity {
    public class Board : MonoBehaviour {
        [Header("Scriptable objects staff")]
        public World world;

        public int level;

        [Header("Board dimensions")]
        public int width;

        public int height;
        public int offset;

        [Header("Prefabs")]
        public GameObject tilePrefab;

        public GameObject breakableTilePrefab;
        public GameObject[] dots;
        public GameObject destroyEffect;

        [Header("Layout")]
        private bool[,] blankSpaces;

        private BackgroundTile[,] breakableTiles;
        public GameObject[,] allDots;
        private FindMatches findMatches;
        public Element currentElement;
        public Tile[] boardLayout;
        public int basePieceValue = 20;
        private int streakValue = 1;
        private ScoreManager scoreManager;
        public float refillDelay = 0.5f;
        public int[] scoreGoals;
        private SoundManager soundManager;
        public GoalManager goalManager;

        public GameState currentState = GameState.MOVE;

        // Start is called before the first frame update
        private void Start() {
            goalManager = FindObjectOfType<GoalManager>();
            soundManager = FindObjectOfType<SoundManager>();
            scoreManager = FindObjectOfType<ScoreManager>();
            breakableTiles = new BackgroundTile[width, height];
            findMatches = FindObjectOfType<FindMatches>();
            blankSpaces = new bool[width, height];
            allDots = new GameObject[width, height];
            currentState = GameState.PAUSE;
            SetUp();
        }

        private void Awake() {
            if (world != null) {
                if (world.levels[level] != null) {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }

        private void GenerateBlankSpaces() {
            for (var i = 0; i < boardLayout.Length; i++) {
                if (boardLayout[i].tileType == TileType.BLANK) {
                    //create a blank space at position
                    blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
                }
            }
        }

        private void GenerateBreakableTiles() {
            //look at all tiles in the layout
            for (var i = 0; i < boardLayout.Length; i++) {
                if (boardLayout[i].tileType == TileType.BREAKABLE) {
                    var temporaryPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                    var tile = Instantiate(breakableTilePrefab, temporaryPosition, Quaternion.identity);
                    breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
                }
            }
        }

        private void SetUp() {
            GenerateBlankSpaces();
            GenerateBreakableTiles();
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (!blankSpaces[i, j]) {
                        var tempPosition = new Vector2(i, j + offset);
                        var tilePosition = new Vector2(i, j);
                        var backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                        backgroundTile.transform.parent = transform;
                        var elementName = "( " + i + ", " + j + " )";
                        backgroundTile.name = elementName;
                        var readyToInsertElement = Random.Range(0, dots.Length);
                        // count of recalculating board elements times
                        var maxIterations = 0;
                        while (MatchesAt(i, j, dots[readyToInsertElement]) && maxIterations < 100) {
                            readyToInsertElement = Random.Range(0, dots.Length);
                            maxIterations++;
                        }

                        maxIterations = 0;

                        var dot = Instantiate(dots[readyToInsertElement], tempPosition, Quaternion.identity);
                        dot.GetComponent<Element>().row = j;
                        dot.GetComponent<Element>().column = i;
                        dot.transform.parent = transform;
                        dot.name = elementName;
                        allDots[i, j] = dot;
                    }
                }
            }
        }

        private bool MatchesAt(int column, int row, GameObject piece) {
            if (column > 1 && row > 1) {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                    if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag)) {
                        return true;
                    }

                    if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) {
                        if (allDots[column, row - 1].CompareTag(piece.tag) &&
                            allDots[column, row - 2].CompareTag(piece.tag)) {
                            return true;
                        }
                    }
                }
            } else {
                if (row > 1) {
                    if (allDots[column, row - 1] != null && allDots[column, row - 2] != null) {
                        if (allDots[column, row - 1].CompareTag(piece.tag) &&
                            allDots[column, row - 2].CompareTag(piece.tag)) {
                            return true;
                        }
                    }
                }

                if (column > 1) {
                    if (allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                        if (allDots[column - 1, row].CompareTag(piece.tag) &&
                            allDots[column - 2, row].CompareTag(piece.tag)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool ColumnOrRow() {
            var numberHorizontal = 0;
            var numberVertical = 0;
            var firstPiece = findMatches.currentMatches[0].GetComponent<Element>();
            if (firstPiece != null) {
                foreach (var currentPiece in findMatches.currentMatches) {
                    var dot = currentPiece.GetComponent<Element>();
                    if (dot.row == firstPiece.row) {
                        numberHorizontal++;
                    }

                    if (dot.column == firstPiece.column) {
                        numberVertical++;
                    }
                }
            }

            return numberVertical == 5 || numberHorizontal == 5;
        }

        private void CheckToMakeBombs() {
            var matchesCount = findMatches.currentMatches.Count;
            if (matchesCount == 4 || matchesCount == 7) {
                findMatches.CheckBombs();
            }

            if (matchesCount == 5 || matchesCount == 6) {
                if (ColumnOrRow()) {
                    //make a color bomb
                    if (currentElement != null) {
                        if (currentElement.isMatched) {
                            if (!currentElement.isColorBomb) {
                                currentElement.isMatched = false;
                                currentElement.MakeColorBomb();
                            }
                        } else {
                            if (currentElement.nextDot != null) {
                                var nextDot = currentElement.nextDot.GetComponent<Element>();
                                if (nextDot.isMatched) {
                                    if (!nextDot.isColorBomb) {
                                        nextDot.isMatched = false;
                                        nextDot.MakeColorBomb();
                                    }
                                }
                            }
                        }
                    }
                } else {
                    //make a adjacent bomb
                    if (currentElement != null) {
                        if (currentElement.isMatched) {
                            if (!currentElement.isAdjacentBomb) {
                                currentElement.isMatched = false;
                                currentElement.MakeAdjacentBomb();
                            }
                        } else {
                            if (currentElement.nextDot != null) {
                                var nextDot = currentElement.nextDot.GetComponent<Element>();
                                if (nextDot.isMatched) {
                                    if (!nextDot.isAdjacentBomb) {
                                        nextDot.isMatched = false;
                                        nextDot.MakeAdjacentBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DestroyMatchesAt(int column, int row) {
            if (allDots[column, row].GetComponent<Element>().isMatched) {
                //check count of matching pieces
                if (findMatches.currentMatches.Count > 3) {
                    CheckToMakeBombs();
                }

                //tile is need to break
                if (breakableTiles[column, row] != null) {
                    breakableTiles[column, row].TakeDamage();
                    if (breakableTiles[column, row].hitPoint <= 0) {
                        breakableTiles[column, row] = null;
                    }
                }

                goalManager.CompareGoal(allDots[column, row].tag);
                goalManager.UpdateGoals();

                soundManager.PlayRandomDestroyNoise();

                var particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
                Destroy(particle, .5f);
                Destroy(allDots[column, row]);
                scoreManager.IncreaseScore(basePieceValue * streakValue);
                allDots[column, row] = null;
            }
        }

        public void DestroyMatches() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (allDots[i, j] != null) {
                        DestroyMatchesAt(i, j);
                    }
                }
            }

            findMatches.currentMatches.Clear();
            StartCoroutine(DecreaseRowCoroutine2());
        }

        private IEnumerator DecreaseRowCoroutine2() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    //current spot isn't blank and is empty
                    if (!blankSpaces[i, j] && allDots[i, j] == null) {
                        //loop from the space above to the top of column
                        for (var k = j + 1; k < height; k++) {
                            //if a dot is found
                            if (allDots[i, k] != null) {
                                //move that element to the empty space
                                allDots[i, k].GetComponent<Element>().row = j;
                                //set that spot to be null
                                allDots[i, k] = null;
                                break;
                            }
                        }
                    }
                }
            }

            //todo убрать задержки
            yield return new WaitForSeconds(refillDelay * 0.5f);
            StartCoroutine(FillBoardCoroutine());
        }

        private void RefillBoard() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (allDots[i, j] == null && !blankSpaces[i, j]) {
                        var tempPosition = new Vector2(i, j + offset);
                        var elementToUse = Random.Range(0, dots.Length);
                        var maxIterations = 0;
                        while (MatchesAt(i, j, dots[elementToUse]) && maxIterations < 100) {
                            maxIterations++;
                            elementToUse = Random.Range(0, dots.Length);
                        }

                        maxIterations = 0;
                        var piece = Instantiate(dots[elementToUse], tempPosition, Quaternion.identity);
                        allDots[i, j] = piece;
                        piece.GetComponent<Element>().row = j;
                        piece.GetComponent<Element>().column = i;
                    }
                }
            }
        }

        private bool MatchesOnBoard() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (allDots[i, j] != null && allDots[i, j].GetComponent<Element>().isMatched) {
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerator FillBoardCoroutine() {
            RefillBoard();
            //todo убрать задержки
            yield return new WaitForSeconds(refillDelay);
            while (MatchesOnBoard()) {
                streakValue++;
                DestroyMatches();
                //todo убрать задержки
                yield return new WaitForSeconds(2 * refillDelay);
            }

            findMatches.currentMatches.Clear();
            currentElement = null;
            if (IsDeadLocked()) {
                ShuffleBoard();
            }

            //todo убрать задержки
            yield return new WaitForSeconds(refillDelay);
            currentState = GameState.MOVE;
            streakValue = 1;
        }

        private void SwitchPieces(int column, int row, Vector2 direction) {
            //take second piece and save it in a holder
            var holder = allDots[column + (int) direction.x, row + (int) direction.y];
            //switching the first element to be a second position
            allDots[column + (int) direction.x, row + (int) direction.y] = allDots[column, row];
            //set the first element to be the second element
            allDots[column, row] = holder;
        }

        private bool CheckForMatches() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (allDots[i, j] != null) {
                        if (i < width - 2) {
                            if (allDots[i + 1, j] != null && allDots[i + 2, j] != null) {
                                if (allDots[i + 1, j].CompareTag(allDots[i, j].tag) &&
                                    allDots[i + 2, j].CompareTag(allDots[i, j].tag)) {
                                    return true;
                                }
                            }
                        }

                        if (j < height - 2) {
                            if (allDots[i, j + 1] != null && allDots[i, j + 2] != null) {
                                if (allDots[i, j + 1].CompareTag(allDots[i, j].tag) &&
                                    allDots[i, j + 2].CompareTag(allDots[i, j].tag)) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool SwitchAndCheck(int column, int row, Vector2 direction) {
            SwitchPieces(column, row, direction);
            if (CheckForMatches()) {
                SwitchPieces(column, row, direction);
                return true;
            }

            SwitchPieces(column, row, direction);
            return false;
        }

        private bool IsDeadLocked() {
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (allDots[i, j] != null) {
                        if (i < width - 1) {
                            if (SwitchAndCheck(i, j, Vector2.right)) {
                                return false;
                            }
                        }

                        if (j < height - 1) {
                            if (SwitchAndCheck(i, j, Vector2.up)) {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void ShuffleBoard() {
            var newBoard = new List<GameObject>();
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (allDots[i, j] != null) {
                        newBoard.Add(allDots[i, j]);
                    }
                }
            }

            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (!blankSpaces[i, j]) {
                        var pieceToUse = Random.Range(0, newBoard.Count);

                        var maxIterations = 0;
                        while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100) {
                            pieceToUse = Random.Range(0, newBoard.Count);
                            maxIterations++;
                        }

                        var piece = newBoard[pieceToUse].GetComponent<Element>();

                        maxIterations = 0;
                        piece.column = i;
                        piece.row = j;
                        allDots[i, j] = newBoard[pieceToUse];
                        newBoard.Remove(newBoard[pieceToUse]);
                    }
                }
            }

            if (IsDeadLocked()) {
                ShuffleBoard();
            }
        }
    }
}