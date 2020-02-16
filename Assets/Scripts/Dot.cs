﻿using System;
using System.Collections;
using UnityEngine;

public class Dot : MonoBehaviour {
    [Header("Board variables")] public int column;
    public int row;
    private int targetX;
    private int targetY;
    public int previousColumn;
    public int previousRow;
    public bool isMatched;

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
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;

    private void Start() {
        isColumnBomb = false;
        isRowBomb = false;
        // works only if there is one board
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    private void Update() {
        TriggerXPosition();
        TriggerYPosition();
    }

    private IEnumerator CheckMoveCoroutine() {
        if (isColorBomb) {
            //this piece is a color bomb and the other piece is the color to destroy
            findMatches.MatchPiecesOfColor(nextDot.tag);
            isMatched = true;
        } else if (nextDot.GetComponent<Dot>().isColorBomb) {
            //the other piece is the color bomb and this piece has the color to destroy
            findMatches.MatchPiecesOfColor(gameObject.tag);
            nextDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if (nextDot != null) {
            var nextDotComponent = nextDot.GetComponent<Dot>();
            if (!isMatched && !nextDotComponent.isMatched) {
                nextDotComponent.row = row;
                nextDotComponent.column = column;
                row = previousRow;
                column = previousColumn;
            }
            else {
                board.DestroyMatches();
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }

            //nextDot = null;
        }
    }

    private void OnMouseDown() {
        if (board.currentState == GameState.move) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp() {
        if (board.currentState == GameState.move) {
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
        }
        else {
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
        }
        else {
            //directly set the position
            tempPosition = new Vector2(transformPosition.x, targetY);
            transform.position = tempPosition;
        }
    }

    private void CalculateAngle() {
        //ignore clicks
        if (Math.Abs(firstTouchPosition.x - finalTouchPosition.x) > SwipeResist ||
            Math.Abs(firstTouchPosition.y - finalTouchPosition.y) > SwipeResist) {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            SwipeElements();
            board.currentState = GameState.wait;
            board.currentDot = this;
        }
        else {
            board.currentState = GameState.move;
        }
    }

    private void SwipeElements() {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            //right swipe
            nextDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            nextDot.GetComponent<Dot>().column--;
            column++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            //up swipe
            nextDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            nextDot.GetComponent<Dot>().row--;
            row++;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            //left swipe
            nextDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            nextDot.GetComponent<Dot>().column++;
            column--;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {
            //down swipe
            nextDot = board.allDots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            nextDot.GetComponent<Dot>().row++;
            row--;
        }

        StartCoroutine(CheckMoveCoroutine());
    }

    public void MakeRowBomb() {
        isRowBomb = true;
        var arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }

    public void MakeColumnBomb() {
        isColumnBomb = true;
        var arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }
}