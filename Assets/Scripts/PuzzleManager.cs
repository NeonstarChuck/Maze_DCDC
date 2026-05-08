using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject door;

    private int greenCount = 0;
    private int redCount = 0;
    private int blueCount = 0;
    private int yellowCount = 0;

    public void PressButton(string color)
    {
        switch (color)
        {
            case "Green":
                greenCount++;
                break;

            case "Red":
                redCount++;
                break;

            case "Blue":
                blueCount++;
                break;

            case "Yellow":
                yellowCount++;
                break;
        }

        Debug.Log(
            "Green: " + greenCount +
            " Red: " + redCount +
            " Blue: " + blueCount +
            " Yellow: " + yellowCount
        );

        CheckSolution();
    }

    void CheckSolution()
    {
        if (
            greenCount == 3 &&
            redCount == 3 &&
            blueCount == 2 &&
            yellowCount == 1
        )
        {
            Debug.Log("PUZZLE SOLVED!");
            door.SetActive(false);
        }
    }
}