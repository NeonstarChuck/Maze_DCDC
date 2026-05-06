using UnityEngine;

public class KeyZone : MonoBehaviour
{
    public Transform leftController;
    public GameObject door;

    public float triggerDistance = 0.25f;
    private bool doorOpened = false;

    void Update()
    {
        if (doorOpened || leftController == null) return;

        // 🔥 Only allow if controller is connected
        if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
            return;

        float dist = Vector3.Distance(leftController.position, transform.position);

        if (dist < triggerDistance)
        {
            Debug.Log("Controller only → door opens");
            door.SetActive(false);
            doorOpened = true;
        }
    }
}