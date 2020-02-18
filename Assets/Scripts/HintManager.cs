using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour {
    private Board board;

    public float hintDelay;

    private float hintDelaySeconds;

    public GameObject hintParticle;

    public GameObject currentHint;

    // Start is called before the first frame update
    private void Start() {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    private void Update() {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null) {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    //find all possible matches on the board
    private List<GameObject> FindAllMatches() {
        var possibleMoves = new List<GameObject>();
        for (var i = 0; i < board.width; i++) {
            for (var j = 0; j < board.height; j++) {
                if (board.allDots[i, j] != null) {
                    if (i < board.width - 1) {
                        if (board.SwitchAndCheck(i, j, Vector2.right)) {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }

                    if (j < board.height - 1) {
                        if (board.SwitchAndCheck(i, j, Vector2.up)) {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    //pick one of them randomly
    private GameObject PickOneRandomly() {
        var possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0) {
            var pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }

        return null;
    }

    //create the hint behind the chosen match
    private void MarkHint() {
        var move = PickOneRandomly();
        if (move != null) {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    //destroy the hint
    public void DestroyHint() {
        if (currentHint != null) {
            Destroy(currentHint);
            currentHint = null;
            //reset timer
            hintDelaySeconds = hintDelay;
        }
    }
}