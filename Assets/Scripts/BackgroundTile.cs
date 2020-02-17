using System;
using UnityEngine;

public class BackgroundTile : MonoBehaviour {

    public int hitPoint;
    private SpriteRenderer sprite;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (hitPoint <= 0) {
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