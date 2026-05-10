using UnityEngine;
using Meta.XR.MRUtilityKit;
using Fusion;

public class QRSpawner : NetworkBehaviour
{
    [System.Serializable]
    public struct QRMapping
    {
        public string qrText;
        public NetworkPrefabRef prefab;
    }

    [Header("QR Configuration")]
    public System.Collections.Generic.List<QRMapping> qrMappings;

    // This dictionary is the "Source of Truth" synced across all headsets
    [Networked] 
    private NetworkDictionary<string, NetworkObject> SpawnedObjects => default;

    // This is called by MRUK (Select the DYNAMIC version in the Inspector!)
    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        // Safety check: Only process QR Codes
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        string payload = trackable.MarkerPayloadString?.Trim();
        if (string.IsNullOrEmpty(payload)) return;

        Debug.Log($"[QR] Local Scan Detected: {payload}");

        // Ask the Host to spawn the object so it's networked for everyone
        RPC_RequestSpawn(payload, trackable.transform.position, trackable.transform.rotation);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestSpawn(string payload, Vector3 pos, Quaternion rot)
    {
        foreach (var mapping in qrMappings)
        {
            if (payload == mapping.qrText.Trim())
            {
                // If we already spawned an object for this QR payload, don't do it again
                if (SpawnedObjects.ContainsKey(payload)) return;

                Debug.Log($"[Fusion] Host Spawning Networked Prefab: {payload}");
                
                // Spawn the networked object
                var spawned = Runner.Spawn(mapping.prefab, pos, rot);
                
                // Add to dictionary so all players know this QR is "active"
                SpawnedObjects.Add(payload, spawned);
                break;
            }
        }
    }

    // Optional: Clean up if trackable is lost
    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        // Usually, we keep the maze even if the QR isn't visible, 
        // but you can add despawn logic here if needed.
    }
}