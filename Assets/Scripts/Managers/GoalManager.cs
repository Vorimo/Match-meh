using System.Collections.Generic;
using System.Linq;
using Entity;
using UnityEngine;

namespace Managers {
    public class GoalManager : MonoBehaviour {
        public GlobalLevelGoal[] globalLevelGoals;
        public GameObject goalPrefab;
        public GameObject goalIntroParent;
        public GameObject goalGameParent;
        public List<LevelStartScreenGoalPart> currentGoalsState = new List<LevelStartScreenGoalPart>();
        public EndGameManager endGameManager;
        private Board board;

        // Start is called before the first frame update
        private void Start() {
            board = FindObjectOfType<Board>();
            endGameManager = FindObjectOfType<EndGameManager>();
            GetGoals();
            SetUpGoals();
        }

        private void GetGoals() {
            globalLevelGoals = board.world.levels[board.level].levelGoals;
            foreach (var levelGoal in globalLevelGoals) {
                levelGoal.numberCollected = 0;
            }
        }

        private void SetUpGoals() {
            foreach (var levelGoal in globalLevelGoals) {
                //goal viewed at start screen level panel
                var startScreenGoal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
                startScreenGoal.transform.SetParent(goalIntroParent.transform, false);
                var goalPart = startScreenGoal.GetComponent<LevelStartScreenGoalPart>();
                goalPart.SetUpImageSprite(levelGoal.goalSprite);
                goalPart.SetUpGoalText(levelGoal.numberNeeded);

                //goal viewed at start screen level panel
                var boardScreenGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
                boardScreenGoal.transform.SetParent(goalGameParent.transform, false);
                goalPart = boardScreenGoal.GetComponent<LevelStartScreenGoalPart>();
                currentGoalsState.Add(goalPart);
                goalPart.SetUpImageSprite(levelGoal.goalSprite);
                goalPart.SetUpGoalText(0, levelGoal.numberNeeded);
            }
        }

        public void UpdateGoals() {
            var goalsCompleted = 0;
            for (var i = 0; i < globalLevelGoals.Length; i++) {
                var collectedElementsCount = globalLevelGoals[i].numberCollected;
                if (globalLevelGoals[i].numberCollected >= globalLevelGoals[i].numberNeeded) {
                    goalsCompleted++;
                    collectedElementsCount = globalLevelGoals[i].numberNeeded;
                }

                currentGoalsState[i].SetUpGoalText(collectedElementsCount, globalLevelGoals[i].numberNeeded);
            }

            //if all goals are completed, we win game
            if (goalsCompleted == globalLevelGoals.Length) {
                endGameManager.WinGame();
            }
        }

        public void CompareGoal(string goalToCompare) {
            globalLevelGoals
                .Where(globalLevelGoal => goalToCompare.Equals(globalLevelGoal.matchValue))
                .ToList()
                .ForEach(globalLevelGoal => globalLevelGoal.numberCollected++);
        }
    }
}