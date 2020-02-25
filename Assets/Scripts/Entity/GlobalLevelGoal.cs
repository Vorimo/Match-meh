using UnityEngine;

namespace Entity {
    [System.Serializable]
    public class GlobalLevelGoal {
        public int numberNeeded;
        public int numberCollected;
        public Sprite goalSprite;
        public string matchValue;
    }
}