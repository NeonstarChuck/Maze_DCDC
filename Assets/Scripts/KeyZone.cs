using UnityEngine;

public class KeyZone : MonoBehaviour
{
    public Transform leftController;
    public RoomProgressManager progressManager;

    public float triggerDistance = 0.25f;
    private bool solved = false;

    void Update()
    {
        if (solved || leftController == null) return;

        if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
            return;

        float dist = Vector3.Distance(leftController.position, transform.position);

        if (dist < triggerDistance)
        {
            Debug.Log("Key puzzle solved");

            solved = true;
            progressManager.KeyPuzzleSolved();
        }
    }
}