using UnityEngine;
using Fusion;

public class QRAnchorManager : NetworkBehaviour
{
    public Transform sharedAnchor;

    public NetworkPrefabRef mazePrefab;

    private bool mazeSpawned = false;

    public void OnQRDetected(Transform qrTransform)
    {
        if (mazeSpawned) return;

        // ONLY HOST SPAWNS
        if (!Runner.IsServer) return;

        // Move anchor
        sharedAnchor.position = qrTransform.position;
        sharedAnchor.rotation = qrTransform.rotation;

        Vector3 mazePos =
            sharedAnchor.position +
            sharedAnchor.forward * 2f;

        // NETWORK SPAWN
        Runner.Spawn(
            mazePrefab,
            mazePos,
            sharedAnchor.rotation
        );

        mazeSpawned = true;

        Debug.Log("Shared maze spawned");
    }
}