using UnityEngine;

public class StartTimerButton : MonoBehaviour
{
    [Header("Drag NetworkedTimer Object Here")]
    public NetworkedTimer targetTimer;

    public void Press()
    {
        if (targetTimer != null)
        {
            targetTimer.RPC_StartTimer();
            Debug.Log("[Button Trigger] StartTimerButton pressed. Sending network request.");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Button clicked but 'targetTimer' slot is completely empty!");
        }
    }
}