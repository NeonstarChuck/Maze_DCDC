using UnityEngine;

public class RoomProgressManager : MonoBehaviour
{
    public Door1 leftDoor;
    public Door1 rightDoor;

    private bool colorSolved = false;
    private bool keySolved = false;

    public void ColorPuzzleSolved()
    {
        colorSolved = true;
        CheckCompletion();
    }

    public void KeyPuzzleSolved()
    {
        keySolved = true;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
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