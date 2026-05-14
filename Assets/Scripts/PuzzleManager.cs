using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public RoomProgressManager progressManager;

    private readonly string[] sequence =
    {
        "Red", "Yellow", "Green", "Blue"
    };

    private int index = 0;
    private bool solved = false;

    public void PressButton(string color)
    {
        if (solved)
            return;

        Debug.Log("Pressed: " + color);
        Debug.Log("Expected: " + sequence[index]);

        if (color == sequence[index])
        {
            index++;

            if (index >= sequence.Length)
            {
                solved = true;
                Debug.Log("Color puzzle solved");

                if (progressManager != null)
                    progressManager.ColorPuzzleSolved();
            }
        }
        else
        {
            Debug.Log("Wrong button. Resetting.");
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        index = 0;
    }
}