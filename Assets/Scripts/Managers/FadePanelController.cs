using System.Collections;
using Entity;
using Enum;
using UnityEngine;

namespace Managers {
    public class FadePanelController : MonoBehaviour {
        public Animator panelAnim;
        public Animator gameInfoAnim;

        public void HandleOkButton() {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStartCoroutine());
        }

        private IEnumerator GameStartCoroutine() {
            yield return new WaitForSeconds(1.5f);
            var board = FindObjectOfType<Board>();
            board.currentState = GameState.MOVE;
        }
    }
}