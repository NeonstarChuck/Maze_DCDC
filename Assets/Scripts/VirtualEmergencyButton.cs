using UnityEngine;

public class VirtualEmergencyButton : MonoBehaviour
{
    [Header("Drag RoomProgressManager Object Here")]
    public RoomProgressManager progressManager;

    public void Press()
    {
        if (progressManager != null)
        {
            progressManager.RPC_RequestEmergencyOverride();
            Debug.Log("[Button Trigger] VirtualEmergencyButton pressed. Sending network request.");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Button clicked but 'progressManager' slot is completely empty!");
        }
    }
}