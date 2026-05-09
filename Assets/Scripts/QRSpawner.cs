using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

public class QRSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct QRMapping
    {
        public string qrText;      // e.g., "MAZE_ROOM_01"
        public GameObject prefab;  // The object to spawn for this text
    }

    [Header("Configuration")]
    public List<QRMapping> qrMappings;

    // Dictionary to keep track of what we spawned for which QR code
    private Dictionary<MRUKTrackable, GameObject> activeSpawns = new Dictionary<MRUKTrackable, GameObject>();

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode && 
            trackable.MarkerPayloadString != null)
        {
            string incomingText = trackable.MarkerPayloadString;
            Debug.Log($"Scanned: {incomingText}");

            // Look through our list to find a match
            foreach (var mapping in qrMappings)
            {
                if (incomingText == mapping.qrText)
                {
                    GameObject newObj = Instantiate(mapping.prefab, trackable.transform.position, trackable.transform.rotation);
                    
                    // Track this specific instance so we can delete it later
                    activeSpawns.Add(trackable, newObj);
                    return; 
                }
            }
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        // If the QR code we were tracking is gone, destroy the object we spawned for it
        if (activeSpawns.ContainsKey(trackable))
        {
            Destroy(activeSpawns[trackable]);
            activeSpawns.Remove(trackable);
        }
    }
}