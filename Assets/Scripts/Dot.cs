using System;
using System.Collections;
using UnityEngine;

public class Dot : MonoBehaviour {
    [Header("Board variables")]
    public int column;

    public int row;
    private int targetX;
    private int targetY;
    public int previousColumn;
    public int previousRow;
    public bool isMatched;

    private HintManager hintManager;
    private FindMatches findMatches;
    public GameObject nextDot;
    private Board board;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    [Header("Swipe stuff")]
    public float swipeAngle;

    private const float SwipeResist = .6f;

    [Header("Powerup stuff")]
    public bool isColumnBomb;

    public bool isRowBomb;
    public bool isColorBomb;
    public bool isAdjacentBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentMarker;

    private void Start() {
        isColumnBomb = false;
        isRowBomb = false;
        isAdjacentBomb = false;

        hintManager = FindObjectOfType<HintManager>();
        // works only if there is one board
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    private void Update() {
        TriggerXPosition();
        TriggerYPosition();
    }

    //This is for testing and Debug only.
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            isColorBomb = true;
            AppendObject(colorBomb);
        }
    }

    private IEnumerator CheckMoveCoroutine() {
        if (isColorBomb) {
            //this piece is a color bomb and the other piece is the color to destroy
            findMatches.MatchPiecesOfColor(gameObject.tag);
            nextDot.GetComponent<Dot>().isMatched = true;
        } else if (nextDot.GetComponent<Dot>().isColorBomb) {
            //the other piece is the color bomb and this piece has the color to destroy
            findMatches.MatchPiecesOfColor(nextDot.tag);
            isMatched = true;
        }

        //todo убрать задержки
        yield return new WaitForSeconds(.5f);
        if (nextDot != null) {
            var nextDotComponent = nextDot.GetComponent<Dot>();
            if (!isMatched && !nextDotComponent.isMatched) {
                nextDotComponent.row = row;
                nextDotComponent.column = column;
                row = previousRow;
                column = previousColumn;
            } else {
                board.DestroyMatches();
                //todo убрать задержки
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.MOVE;
            }

            //nextDot = null;
        }
    }

    private void OnMouseDown() {
        //destroy the hint if exists
        if (hintManager != null) {
            hintManager.DestroyHint();
        }

        if (board.currentState == GameState.MOVE) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp() {
        if (board.currentState == GameState.MOVE) {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void TriggerXPosition() {
        targetX = column;
        var transformPosition = transform.position;
        if (Mathf.Abs(targetX - transformPosition.x) > .1) {
            //move towards the target
            tempPosition = new Vector2(targetX, transformPosition.y);
            transform.position = Vector3.Lerp(transformPosition, tempPosition, .6f);
            if (board.allDots[column, row] != gameObject) {
                board.allDots[column, row] = gameObject;
            }

            findMatches.FindAllMatches();
        } else {
            //directly set the position
            tempPosition = new Vector2(targetX, transformPosition.y);
            transform.position = tempPosition;
        }
    }

    private void TriggerYPosition() {
        targetY = row;
        var transformPosition = transform.position;
        if (Mathf.Abs(targetY - transformPosition.y) > .1) {
            //move towards the target
            tempPosition = new Vector2(transformPosition.x, targetY);
            transform.position = Vector3.Lerp(transformPosition, tempPosition, .6f);
            if (board.allDots[column, row] != gameObject) {
                board.allDots[column, row] = gameObject;
            }

            findMatches.FindAllMatches();
        } else {
            //directly set the position
            tempPosition = new Vector2(transformPosition.x, targetY);
            transform.position = tempPosition;
        }
    }

    private void CalculateAngle() {
        //ignore clicks
        if (Math.Abs(firstTouchPosition.x - finalTouchPosition.x) > SwipeResist ||
            Math.Abs(firstTouchPosition.y - finalTouchPosition.y) > SwipeResist) {
            board.currentState = GameState.WAIT;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            SwipeElements();
            board.currentDot = this;
        } else {
            board.currentState = GameState.MOVE;
        }
    }

    private void MovePiecesActual(Vector2 direction) {
        nextDot = board.allDots[column + (int) direction.x, row + (int) direction.y];
        previousRow = row;
        previousColumn = column;
        if (nextDot != null) {
            nextDot.GetComponent<Dot>().column -= (int) direction.x;
            nextDot.GetComponent<Dot>().row -= (int) direction.y;
            column += (int) direction.x;
            row += (int) direction.y;
            StartCoroutine(CheckMoveCoroutine());
        } else {
            board.currentState = GameState.MOVE;
        }
    }

    private void SwipeElements() {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            //right swipe
            MovePiecesActual(Vector2.right);
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            //up swipe
            MovePiecesActual(Vector2.up);
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            //left swipe
            MovePiecesActual(Vector2.left);
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {
            //down swipe
            MovePiecesActual(Vector2.down);
        } else {
            board.currentState = GameState.MOVE;
        }
    }

    public void MakeRowBomb() {
        isRowBomb = true;
        AppendObject(rowArrow);
    }

    public void MakeColumnBomb() {
        isColumnBomb = true;
        AppendObject(columnArrow);
    }

    public void MakeColorBomb() {
        isColorBomb = true;
        AppendObject(colorBomb);
    }

    public void MakeAdjacentBomb() {
        isColorBomb = true;
        AppendObject(adjacentMarker);
    }

    private void AppendObject(GameObject appendableObject) {
        var appendedObject = Instantiate(appendableObject, transform.position, Quaternion.identity);
        appendedObject.transform.parent = transform;
    }
}