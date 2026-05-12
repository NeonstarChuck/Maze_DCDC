using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Door1 stage1Door;
    public Door1 stage2Door;

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
        {
            Debug.Log("All stages completed!");
            return;
        }

        Debug.Log("Pressed: " + color);

        if (color == sequences[currentStage][index])
        {
            index++;
            Debug.Log("Correct step: " + index + "/" + sequences[currentStage].Length);

            if (index >= sequences[currentStage].Length)
            {
                Debug.Log("STAGE " + (currentStage + 1) + " SOLVED!");

                OpenCurrentDoor();

                currentStage++;
                ResetPuzzle();
            }
        }
        else
        {
            Debug.Log("WRONG! Resetting puzzle");
            ResetPuzzle();
        }
    }

    private void OpenCurrentDoor()
    {
        if (currentStage == 0 && stage1Door != null)
        {
            stage1Door.Open();
        }
        else if (currentStage == 1 && stage2Door != null)
        {
            stage2Door.Open();
        }
    }

    private void ResetPuzzle()
    {
        index = 0;
    }
}