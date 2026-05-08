using UnityEngine;

public class TableZone : MonoBehaviour
{
    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            Debug.Log("KEY ENTERED ZONE → hiding door");
            door.SetActive(false);
        }
    }
}