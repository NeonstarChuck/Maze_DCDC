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

    [Networked]
    private NetworkDictionary<string, NetworkObject> SpawnedObjects => default;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        // 🚨 HOST ONLY GUARD 🚨
        // If this headset is a client, instantly drop out. Only the Host's room matters!
        if (!Object.HasStateAuthority) return;

        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode)
            return;

        string payload = trackable.MarkerPayloadString?.Trim();
        if (string.IsNullOrEmpty(payload))
            return;

        Debug.Log($"[QR Host Scan] Detected: '{payload}'");

        RPC_RequestSpawn(payload, trackable.transform.position, trackable.transform.rotation);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestSpawn(string payload, Vector3 pos, Quaternion rot)
    {
        payload = payload.Trim();

        foreach (var mapping in qrMappings)
        {
            if (!string.Equals(payload, mapping.qrText.Trim(), System.StringComparison.OrdinalIgnoreCase))
                continue;

            if (SpawnedObjects.ContainsKey(payload))
            {
                Debug.Log($"Already spawned: {payload}");
                return;
            }

            Debug.Log($"Spawning 3D Number Model: {payload}");

            // Apply 180° rotation on X axis
            Quaternion fixedRot = rot * Quaternion.Euler(180f, 0f, 0f);

            NetworkObject spawned = Runner.Spawn(mapping.prefab, pos, fixedRot);

            SpawnedObjects.Set(payload, spawned);
            return;
        }

        Debug.LogWarning($"No mapping found for QR: {payload}");
    }

    // --- 🚨 THE MASTER MAZE RESET LINK 🚨 ---
    // Called by the RoomProgressManager when the master 'B' button is clicked
    public void ResetQRSpawnerState()
    {
        if (!Object.HasStateAuthority) return;

        Debug.Log("[QRSpawner] Master reset received. Despawning all 3D number objects...");

        // 1. Collect all spawned items into a temporary list to safely avoid enumeration crashes
        System.Collections.Generic.List<NetworkObject> objectsToDespawn = new System.Collections.Generic.List<NetworkObject>();
        foreach (var kvp in SpawnedObjects)
        {
            if (kvp.Value != null)
            {
                objectsToDespawn.Add(kvp.Value);
            }
        }

        // 2. Erase them from the multiplayer network universe entirely
        foreach (var networkObj in objectsToDespawn)
        {
            if (networkObj != null)
            {
                Runner.Despawn(networkObj);
            }
        }

        // 3. Completely wipe the memory dictionary clean for the next speedrun
        SpawnedObjects.Clear();
        Debug.Log("[QRSpawner] Wipe complete. Ready for re-scanning.");
    }
}