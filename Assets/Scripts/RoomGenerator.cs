using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    private int currentRoomIndex = 0;
    private GameObject activeRoom;

    void Start()
    {
        SpawnRoom(0);
    }

    public void SpawnNextRoom()
    {
        int nextIndex = currentRoomIndex + 1;

        if (nextIndex >= roomPrefabs.Length)
        {
            Debug.Log("No more rooms");
            return;
        }

        SpawnRoom(nextIndex);
    }

    private void SpawnRoom(int index)
    {
        // destroy old room
        if (activeRoom != null)
            Destroy(activeRoom);

        currentRoomIndex = index;

        // spawn new room
        activeRoom = Instantiate(roomPrefabs[currentRoomIndex]);

        Debug.Log("Spawned room: " + currentRoomIndex);
    }
}