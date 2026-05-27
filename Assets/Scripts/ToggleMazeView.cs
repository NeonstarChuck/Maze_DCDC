using UnityEngine;

public class ToggleMazeView : MonoBehaviour
{
    [Header("Drag Maze Walls or Room Objects Here")]
    public GameObject[] mazeObjectsToToggle;

    private bool isMazeHidden = false;

    public void Press()
    {
        // Flip the true/false state every time the button is clicked
        isMazeHidden = !isMazeHidden;

        // Loop through all the assigned objects and toggle them
        foreach (GameObject obj in mazeObjectsToToggle)
        {
            if (obj != null)
            {
                // If isMazeHidden is true, SetActive becomes false (hides object)
                // If isMazeHidden is false, SetActive becomes true (shows object)
                obj.SetActive(!isMazeHidden);
            }
        }

        Debug.Log($"[Local Action] Maze toggled! Hidden for this player: {isMazeHidden}. Other players are unaffected.");
    }
}