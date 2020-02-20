using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal {
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour {
    public BlankGoal[] levelGoals;
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    // Start is called before the first frame update
    private void Start() {
        SetUpGoals();
    }

    private void SetUpGoals() {
        for (var i = 0; i < levelGoals.Length; i++) {
            //create a new goal panel
            var goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform, false);
            var panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            //todo without string
            panel.thisString = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            var gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            //todo without string
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals() {
        var goalsCompleted = 0;
        for (var i = 0; i < levelGoals.Length; i++) {
            currentGoals[i].thisText.text = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded) {
                goalsCompleted++;
                //todo
                currentGoals[i].thisText.text = levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
    }

    public void CompareGoal(string goalToCompare) {
        for (var i = 0; i < levelGoals.Length; i++) {
            if (goalToCompare == levelGoals[i].matchValue) {
                levelGoals[i].numberCollected++;
            }
        }
    }
}