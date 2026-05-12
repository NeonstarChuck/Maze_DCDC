using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public RoomProgressManager progressManager;

    private readonly string[][] sequences =
    {
        new string[] { "Red", "Yellow", "Green", "Blue" },
        new string[] { "Blue", "Green", "Yellow", "Red" }
    };

    private int currentStage = 0;
    private int index = 0;

    public void PressButton(string color)
    {
        if (currentStage >= sequences.Length)
            return;

        if (color == sequences[currentStage][index])
        {
            index++;

            if (index >= sequences[currentStage].Length)
            {
                currentStage++;
                ResetPuzzle();

                if (currentStage >= sequences.Length)
                {
                    Debug.Log("Color puzzle solved");
                    progressManager.ColorPuzzleSolved();
                }
            }
        }
        else
        {
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        index = 0;
    }
}