using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Door1 door;

    private string[] sequence = new string[]
    {
        "Yellow",
        "Yellow",
        "Blue",
        "Red",
        "Red",
        "Red",
        "Green"
    };

    private int index = 0;

    public void PressButton(string color)
    {
        Debug.Log("Pressed: " + color);

        // correct step
        if (color == sequence[index])
        {
            index++;
            Debug.Log("Correct step: " + index + "/" + sequence.Length);

            if (index >= sequence.Length)
            {
                Debug.Log("PUZZLE SOLVED!");
                door.Open();
                ResetPuzzle();
            }
        }
        else
        {
            Debug.Log("WRONG! Resetting puzzle");
            ResetPuzzle();
        }
    }

    void ResetPuzzle()
    {
        index = 0;
    }
}