using UnityEngine;
using UnityEngine.UI;

namespace Entity {
    public class LevelStartScreenGoalPart : MonoBehaviour {
        public Image partImage;
        public Text partText;

        public void SetUpImageSprite(Sprite sprite) {
            partImage.sprite = sprite;
        }

        public void SetUpGoalText(int value) {
            partText.text = "" + value;
        }

        public void SetUpGoalText(int currentValue, int valueNeeded) {
            partText.text = currentValue + "/" + valueNeeded;
        }
    }
}