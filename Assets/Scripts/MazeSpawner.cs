using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject goalPrefab;

    public float spacing = 1.5f;
    private GameObject goalInstance;

    void Start()
    {
        SpawnMaze();
    }

    void SpawnMaze()
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        Quaternion mazeRotation = Quaternion.LookRotation(forward.normalized);

        Vector3 center = Camera.main.transform.position + forward.normalized * 2.5f;
        center.y = 0f;

        int[,] layout =
        {
            {1, 1, 1}, // Top Row (Back Wall)
            {1, 0, 1}, // Mid Row (Arms)
            {1, 0, 1}  // Bottom Row (Arms)
        };

        int rows = layout.GetLength(0);
        int cols = layout.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (layout[r, c] == 0) continue;

                float xOffset = (c - (cols - 1) / 2f) * spacing;
                float zOffset = ((rows - 1 - r) - (rows - 1) / 2f) * spacing;

                // --- THE NUDGE ---
                // If it's the back-middle wall, push it back half its thickness (0.1f)
                if (r == 0 && c == 1) {
                    zOffset += 0.1f; 
                }

                Vector3 localPos = new Vector3(xOffset, 0, zOffset);
                Vector3 finalWorldPos = center + (mazeRotation * localPos);

                Quaternion wallRot = mazeRotation; 

                // Rotate side arms
                if (c == 0 || c == cols - 1) {
                    wallRot *= Quaternion.Euler(0, 90, 0);
                }
                
                // Keep back wall horizontal
                if (r == 0 && c == 1) {
                    wallRot = mazeRotation;
                }

                GameObject wall = Instantiate(wallPrefab, finalWorldPos, wallRot);

                // --- THE OVERLAP ---
                // Stretch the wall slightly (0.2f extra) so corners meet perfectly
                Vector3 s = wall.transform.localScale;
                wall.transform.localScale = new Vector3(spacing + 0.2f, s.y, s.z);
            }
        }

        // Goal placement
        float goalZ = ((rows - 1 - (rows - 1)) - (rows - 1) / 2f) * spacing;
        goalInstance = Instantiate(goalPrefab, center + (mazeRotation * new Vector3(0, 0, goalZ)), mazeRotation);
    }

    void Update()
    {
        if (goalInstance == null) return;
        if (Vector3.Distance(Camera.main.transform.position, goalInstance.transform.position) < 0.6f)
        {
            Debug.Log("YOU WIN 🎉");
        }
    }
}
