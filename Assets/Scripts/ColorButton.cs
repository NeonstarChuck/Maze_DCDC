using UnityEngine;

public class ColorButton : MonoBehaviour
{
    public string colorName;
    public PuzzleManager manager;

    public void Press()
    {
        manager.PressButton(colorName);
        Debug.Log(colorName + " pressed");
    }
}