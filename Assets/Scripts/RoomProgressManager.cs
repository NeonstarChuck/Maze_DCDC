using UnityEngine;

public class RoomProgressManager : MonoBehaviour
{
    public Door1 leftDoor;
    public Door1 rightDoor;

    private bool colorSolved = false;
    private bool keySolved = false;

    public void ColorPuzzleSolved()
    {
        Debug.Log("Color puzzle complete");
        colorSolved = true;
        CheckCompletion();
    }

    public void KeyPuzzleSolved()
    {
        Debug.Log("Key puzzle complete");
        keySolved = true;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        Debug.Log("Checking completion...");

        if (colorSolved && keySolved)
        {
            Debug.Log("Both puzzles solved. Opening both doors.");

            if (leftDoor != null)
                leftDoor.Open();

            if (rightDoor != null)
                rightDoor.Open();
        }
    }
}