using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState {
    wait,
    move
}

public class Board : MonoBehaviour {
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public FindMatches findMatches;
    public GameObject destroyEffect;
    public Dot currentDot;

    // Start is called before the first frame update
    private void Start() {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    private void SetUp() {
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                var tempPosition = new Vector2(i, j);
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
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
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = transform;
                dot.name = elementName;
                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) {
        if (column > 1 && row > 1) {
            if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag)
                || allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag)) {
                return true;
            }
        }
        else {
            if (row > 1 && allDots[column, row - 1].CompareTag(piece.tag) &&
                allDots[column, row - 2].CompareTag(piece.tag)) {
                return true;
            }

            if (column > 1 && allDots[column - 1, row].CompareTag(piece.tag) &&
                allDots[column - 2, row].CompareTag(piece.tag)) {
                return true;
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row) {
        if (allDots[column, row].GetComponent<Dot>().isMatched) {
            //check count of matching pieces
            if (findMatches.currentMatches.Count > 3) {
                findMatches.CheckBombs();
            }
            findMatches.currentMatches.Remove(allDots[column, row]);
            var particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
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

        StartCoroutine(DecreaseRowCoroutine());
    }

    private IEnumerator DecreaseRowCoroutine() {
        var emptySpacesCount = 0;
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (allDots[i, j] == null) {
                    emptySpacesCount++;
                }
                else if (emptySpacesCount > 0) {
                    allDots[i, j].GetComponent<Dot>().row -= emptySpacesCount;
                    allDots[i, j] = null;
                }
            }

            emptySpacesCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    private void RefillBoard() {
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (allDots[i, j] == null) {
                    var tempPosition = new Vector2(i, j + offset);
                    var elementToUse = Random.Range(0, dots.Length);
                    var piece = Instantiate(dots[elementToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard() {
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched) {
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoardCoroutine() {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard()) {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }

        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}