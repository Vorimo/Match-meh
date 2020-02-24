using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject {
    [Header("Board dimensions")]
    public int width;

    public int height;

    [Header("Starting tiles")]
    public TileType[] boardLayout;

    [Header("Available elements")]
    public GameObject[] dots;

    public int[] scoreGoals;

    [Header("End game requirements")]
    public EndGameRequirements endGameRequirements;

    public BlankGoal[] levelGoals;
}