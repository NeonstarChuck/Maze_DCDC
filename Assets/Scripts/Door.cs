using UnityEngine;

public class StartHideTest : MonoBehaviour
{
    public GameObject door;

    void Start()
    {
        if (door == null)
        {
            Debug.LogError("Door NOT assigned!");
            return;
        }

        Debug.Log("Hiding door on Start");
        door.SetActive(false);
    }
}