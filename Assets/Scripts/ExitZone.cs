using UnityEngine;
using System.Collections.Generic;

public class ExitZone : MonoBehaviour
{
    public RoomGenerator roomGenerator;

    private HashSet<GameObject> playersInside = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playersInside.Add(other.gameObject);

        if (playersInside.Count >= 2)
        {
            Debug.Log("Both players entered transition zone");
            roomGenerator.SpawnNextRoom();
        }
    }
}