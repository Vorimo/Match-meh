using Entity;
using Managers;
using UnityEngine;

namespace Scriptable_Objects {
    [CreateAssetMenu(fileName = "Level", menuName = "Level")]
    public class Level : ScriptableObject {
        [Header("Board dimensions")]
        public int width;

        public int height;

        [Header("Starting tiles")]
        public Tile[] boardLayout;

        [Header("Available elements")]
        public GameObject[] dots;

        public int[] scoreGoals;

        [Header("End game requirements")]
        public EndGameRequirementState endGameRequirementState;

        public GlobalLevelGoal[] levelGoals;
    }
}