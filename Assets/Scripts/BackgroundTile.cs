using UnityEngine;

public class BackgroundTile : MonoBehaviour {

    public int hitPoint;
    private SpriteRenderer sprite;
    private GoalManager goalManager;

    private void Start() {
        goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (hitPoint <= 0) {
            if (goalManager != null) {
                goalManager.CompareGoal(gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage) {
        hitPoint -= damage;
        MakeLighter();
    }

    private void MakeLighter() {
        //take the current color
        var color = sprite.color;
        //get the current color's alpha value
        var newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}