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

    // SAFE for your Fusion version
    [Networked]
    private NetworkDictionary<string, NetworkObject> SpawnedObjects => default;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode)
            return;

        string payload = trackable.MarkerPayloadString?.Trim();
        if (string.IsNullOrEmpty(payload))
            return;

        Debug.Log($"[QR] Detected: '{payload}'");

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

            Debug.Log($"Spawning: {payload}");

            // 🔥 FIX: Apply 180° rotation on X axis
            Quaternion fixedRot = rot * Quaternion.Euler(180f, 0f, 0f);

            NetworkObject spawned = Runner.Spawn(mapping.prefab, pos, fixedRot);

            SpawnedObjects.Set(payload, spawned);

            return;
        }

        Debug.LogWarning($"No mapping found for QR: {payload}");
    }
}