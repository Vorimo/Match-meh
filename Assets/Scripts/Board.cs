using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState {
    WAIT,
    MOVE
}

public enum TileKind {
    BREAKABLE,
    BLANK,
    NORMAL
}

[System.Serializable]
public class TileType {
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour {
    public GameState currentState = GameState.MOVE;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    private bool[,] blankSpaces;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public FindMatches findMatches;
    public GameObject destroyEffect;
    public Dot currentDot;
    public TileType[] boardLayout;

    // Start is called before the first frame update
    private void Start() {
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    public void GenerateBlankSpaces() {
        for (var i = 0; i < boardLayout.Length; i++) {
            if (boardLayout[i].tileKind == TileKind.BLANK) {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    private void SetUp() {
        GenerateBlankSpaces();
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (!blankSpaces[i, j]) {
                    var tempPosition = new Vector2(i, j + offset);
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
        }
        else {
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

    public bool ColumnOrRow() {
        var numberHorizontal = 0;
        var numberVertical = 0;
        var firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null) {
            foreach (var currentPiece in findMatches.currentMatches) {
                var dot = currentPiece.GetComponent<Dot>();
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
                if (currentDot != null) {
                    if (currentDot.isMatched) {
                        if (!currentDot.isColorBomb) {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else {
                        if (currentDot.nextDot != null) {
                            var nextDot = currentDot.nextDot.GetComponent<Dot>();
                            if (nextDot.isMatched) {
                                if (!nextDot.isColorBomb) {
                                    nextDot.isMatched = false;
                                    nextDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else {
                //make a adjacent bomb
                if (currentDot != null) {
                    if (currentDot.isMatched) {
                        if (!currentDot.isAdjacentBomb) {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else {
                        if (currentDot.nextDot != null) {
                            var nextDot = currentDot.nextDot.GetComponent<Dot>();
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
        if (allDots[column, row].GetComponent<Dot>().isMatched) {
            //check count of matching pieces
            if (findMatches.currentMatches.Count > 3) {
                CheckToMakeBombs();
            }

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
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //set that spot to be null
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }

        //todo убрать задержки
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCoroutine());
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

        //todo убрать задержки
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    private void RefillBoard() {
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                if (allDots[i, j] == null && !blankSpaces[i, j]) {
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
        //todo убрать задержки
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard()) {
            //todo убрать задержки
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }

        findMatches.currentMatches.Clear();
        currentDot = null;
        //todo убрать задержки
        yield return new WaitForSeconds(.5f);
        currentState = GameState.MOVE;
    }
}