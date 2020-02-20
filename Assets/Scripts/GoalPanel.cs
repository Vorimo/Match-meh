﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour {
    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;

    // Start is called before the first frame update
    private void Start() {
        SetUp();
    }

    private void SetUp() {
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }

    // Update is called once per frame
    void Update() {
    }
}